﻿using System.Linq;
using ZBuildLights.Core.Models;

namespace ZBuildLights.Core.Services
{
    public class UnassignedLightService : IUnassignedLightService
    {
        private readonly IZWaveNetwork _network;

        public UnassignedLightService(IZWaveNetwork network)
        {
            _network = network;
        }

        public void SetUnassignedLights(MasterModel masterModel)
        {
            var allLights = masterModel.AllLights;
            var allSwitches = _network.GetAllSwitches();

            var switchesAlreadyInAProject = allSwitches
                .Where(sw =>
                    allLights.Any(l =>
                        l.ZWaveDeviceId.Equals(sw.DeviceId) &&
                        l.ZWaveHomeId.Equals(sw.HomeId)
                        )
                ).ToArray();

            var newSwitches = allSwitches.Except(switchesAlreadyInAProject);
            var newLights = newSwitches.Select(x => new Light(x.HomeId, x.DeviceId)).ToArray();

            masterModel.AddUnassignedLights(newLights);
        }
    }
}