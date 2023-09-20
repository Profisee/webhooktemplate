const express = require("express");
const router = require("./controllers/router");
const { PORT } = require("./config");

const app = express();

app.use(express.json());
app.use("/webhooks", router);

app.listen(PORT, () => {
  console.log(`listening on port ${PORT}`);
});
