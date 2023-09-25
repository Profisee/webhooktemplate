const crypto = require("crypto");
const jwt = require("jsonwebtoken");
const { SERVICE_URL } = require("../config.js");

const OPENID_CONFIGURATION_URL = `${SERVICE_URL}auth/.well-known/openid-configuration`;
const JWKS_URL = `${OPENID_CONFIGURATION_URL}/jwks`;

async function validateAndDecodeJwt(authHeader) {
  const token = authHeader.replace("Bearer ", "");

  const discoveryResponse = await fetch(OPENID_CONFIGURATION_URL);
  if (!discoveryResponse.ok) {
    throw new Error(
      `Error fetching OpenID configuration: ${discoveryResponse.status} ${discoveryResponse.statusText}`
    );
  }
  const discoveryDocument = await discoveryResponse.json();

  const jwksResponse = await fetch(JWKS_URL);
  if (!jwksResponse.ok) {
    throw new Error(`Error fetching JWKS: ${jwksResponse.status} ${jwksResponse.statusText}`);
  }
  const jwks = await jwksResponse.json();

  const rsaKey = jwks.keys.find((key) => key.kty === "RSA");
  if (!rsaKey) {
    throw new Error("RSA public key not found in JWKS");
  }

  let publicKey = crypto.createPublicKey({
    key: rsaKey,
    format: "jwk",
  });

  const validationParams = { issuer: discoveryDocument.issuer };
  const decodedToken = jwt.verify(token, publicKey, validationParams);

  return decodedToken;
}

module.exports = validateAndDecodeJwt;
