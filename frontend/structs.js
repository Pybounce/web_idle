function createStartResourceHarvestMessage(resourceNodeId) {
  return JSON.stringify({
    messageType: "StartResourceHarvest",
    data: {
      resourceNodeId: resourceNodeId,
    },
  });
}

function createStopResourceHarvestMessage(resourceNodeId) {
  return JSON.stringify({
    messageType: "StopResourceHarvest",
    data: {
      resourceNodeId: resourceNodeId,
    },
  });
}
