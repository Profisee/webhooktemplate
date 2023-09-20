const { validateAndDecodeJwt } = require("../services");

async function workflowActivityController(req, res) {
  let message;

  // get auth header from request
  const authHeader = req.headers.authorization;

  // validate and decode token
  let decodedToken;
  try {
    decodedToken = await validateAndDecodeJwt(authHeader);
  } catch (err) {
    message = `JWT is not valid: ${err.message}`;
    console.error(message);
    return res.status(200).json({ ProcessingStatus: -1, RequestPayload: { message } });
  }

  // log received data
  console.log("Data received from workflow activity: ", req.body, "\n");

  return res
    .status(200)
    .json({ ProcessingStatus: 0, ResponsePayload: { message: "webhook completed successfully" } });
}

module.exports = workflowActivityController;
