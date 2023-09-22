const { validateAndDecodeJwt, updateRecordDescription } = require("../services");

async function workflowActivityController(req, res) {
  let message;

  // get auth header from request
  const authHeader = req.headers.authorization;
  if (!authHeader) {
    message = "NO Authorization header";
    console.error(message);
    return res.status(401).json({ ProcessingStatus: -1, ResponsePayload: { message } });
  }

  // validate and decode token
  let decodedToken;
  try {
    decodedToken = await validateAndDecodeJwt(authHeader);
  } catch (err) {
    message = `JWT is not valid: ${err.message}`;
    console.error(message);
    return res.status(403).json({ ProcessingStatus: -1, ResponsePayload: { message } });
  }

  // check for required data
  const { EntityId, Code } = req.body;
  if (!EntityId || !Code) {
    message = "Body is undefined or empty";
    console.error(message);
    return res.status(200).json({ ProcessingStatus: -1, ResponsePayload: { message } });
  }

  // update record
  const updatedRecord = `EntityID: ${EntityId} - Code: ${Code}`;
  const timestamp = new Date().toLocaleString();
  const newDescription = `${updatedRecord} was updated by the Node.js Webhook using the Profisee Rest API at ${timestamp}`;
  try {
    updateRecordDescription({
      entityId: EntityId,
      memberCode: Code,
      newDescription,
      authHeader,
    });
  } catch (err) {
    message = `Error updating record: ${err.message}`;
    console.error(message);
    return res.status(200).json({ ProcessingStatus: -1, ResponsePayload: { message } });
  }
  console.log(newDescription);

  return res
    .status(200)
    .json({ ProcessingStatus: 0, ResponsePayload: { message: newDescription } });
}

module.exports = workflowActivityController;
