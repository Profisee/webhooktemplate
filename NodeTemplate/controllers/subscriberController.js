const { validateAndDecodeJwt, updateRecordDescription } = require("../services");

async function subscriberController(req, res) {
  let message;

  // get auth header from request
  const authHeader = req.headers.authorization;
  if (!authHeader) {
    message = "Authorization header not found in the request";
    console.error(message);
    return res.status(200).json({ ProcessingStatus: -1, ResponsePayload: { message } });
  }

  // validate and decode token
  let decodedToken;
  try {
    decodedToken = await validateAndDecodeJwt(authHeader);
  } catch (err) {
    message = `JWT is not valid: ${err.message}`;
    console.error(message);
    return res.status(200).json({ ProcessingStatus: -1, ResponsePayload: { message } });
  }

  // check for required data
  console.log(req.body);
  const { entityObject, membercode } = req.body;
  if (!entityObject || !membercode) {
    message = "expected data not found in the request";
    console.error(message);
    return res.status(200).json({ ProcessingStatus: -1, RequestPayload: { message } });
  }

  // update record
  const updatedRecord = `${entityObject.Name} - ${membercode}`;
  const timestamp = new Date().toLocaleString();
  const newDescription = `${updatedRecord} was updated by the Node.js Webhook using the Profisee Rest API at ${timestamp}`;
  try {
    updateRecordDescription({ entityObject, membercode, newDescription, authHeader });
  } catch (err) {
    message = `Error updating record: ${err.message}`;
    console.error(message);
    return res.status(200).json({ ProcessingStatus: -1, RequestPayload: { message } });
  }
  console.log(newDescription);

  return res
    .status(200)
    .json({ ProcessingStatus: 0, ResponsePayload: { message: newDescription } });
}

module.exports = subscriberController;
