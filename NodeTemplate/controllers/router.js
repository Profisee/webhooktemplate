const express = require("express");
const subscriberController = require("./subscriberController");
const workflowActivityController = require("./workflowActivityController");

const router = express.Router();

router.post("/subscriber", subscriberController);
router.post("/workflow-activity", workflowActivityController);

module.exports = router;
