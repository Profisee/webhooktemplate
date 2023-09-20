const { SERVICE_URL } = require("../config.js");

const API_URL = `${SERVICE_URL}rest/v1`;

async function updateRecordDescription({ entityObject, membercode, newDescription, authHeader }) {
  const headers = {
    authorization: authHeader,
    "Content-Type": "application/json",
  };

  const res = await fetch(`${API_URL}/Records/${entityObject.Id}/${membercode}`, {
    method: "PATCH",
    body: JSON.stringify({ Description: newDescription }),
    headers,
  });
  if (!res.ok) {
    throw new Error(`Error updating record: ${res.status} ${res.statusText}`);
  }
}

module.exports = updateRecordDescription;
