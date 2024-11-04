function createStartResourceHarvestMessage(resourceId) {
  return JSON.stringify({
    messageType: "StartResourceHarvest",
    data: {
      resourceId: Number(resourceId),
    },
  });
}

function createStopResourceHarvestMessage(resourceId) {
  return JSON.stringify({
    messageType: "StopResourceHarvest",
    data: {
      resourceId: Number(resourceId),
    },
  });
}
