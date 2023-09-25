const { SERVICE_URL } = require("../config.js");

const API_URL = `${SERVICE_URL}rest/v1`;

async function updateRecordDescription({ entityId, memberCode, newDescription, authHeader }) {
  const headers = {
    authorization: authHeader,
    "Content-Type": "application/json",
  };

  const res = await fetch(`${API_URL}/Records/${entityId}/${memberCode}`, {
    method: "PATCH",
    body: JSON.stringify({ Description: newDescription }),
    headers,
  });
  if (!res.ok) {
    throw new Error(`Error updating record: ${res.status} ${res.statusText}`);
  }
}

module.exports = updateRecordDescription;
