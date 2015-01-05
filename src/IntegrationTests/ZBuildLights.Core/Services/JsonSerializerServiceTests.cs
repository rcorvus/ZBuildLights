﻿using System;
using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using Should;
using ZBuildLights.Core.Enumerations;
using ZBuildLights.Core.Models;
using ZBuildLights.Core.Services.Storage;
using ZBuildLights.Web;
using ZBuildLights.Web.DependencyResolution;

namespace IntegrationTests.ZBuildLights.Core.Services
{
    public class JsonSerializerServiceTests
    {
        [TestFixture]
        public class When_working_with_the_master_model_object
        {
            [Test]
            public void Should_serialize_and_deserialize()
            {
                AutoMapperConfig.Initialize();
                var container = IoC.Initialize();

                var masterModel = new MasterModel {LastUpdatedDate = DateTime.Now};
                var core = masterModel.CreateProject(x =>
                {
                    x.Name = "Core";
                    x.StatusMode = StatusMode.Success;
                });
                core.AddGroup(new LightGroup {Name = "SnP Square", Id = Guid.NewGuid()})
                    .AddLight(new Light(1, 1) {Color = LightColor.Green, SwitchState = SwitchState.On})
                    .AddLight(new Light(1, 2) {Color = LightColor.Yellow, SwitchState = SwitchState.Off})
                    .AddLight(new Light(1, 3) {Color = LightColor.Red, SwitchState = SwitchState.Off})
                    ;
                core.AddGroup(new LightGroup {Name = "SnP Near Matt", Id = Guid.NewGuid()})
                    .AddLight(new Light(1, 4) {Color = LightColor.Green, SwitchState = SwitchState.On})
                    .AddLight(new Light(1, 5) {Color = LightColor.Yellow, SwitchState = SwitchState.Off})
                    .AddLight(new Light(1, 6) {Color = LightColor.Red, SwitchState = SwitchState.Off})
                    ;

                var apps = masterModel.CreateProject(x =>
                {
                    x.StatusMode = StatusMode.BrokenAndBuilding;
                    x.Name = "Apps";
                });
                apps.AddGroup(new LightGroup {Name = "Near Window", Id = Guid.NewGuid()})
                    .AddLight(new Light(1, 7) {Color = LightColor.Green, SwitchState = SwitchState.Off})
                    .AddLight(new Light(1, 8) {Color = LightColor.Yellow, SwitchState = SwitchState.On})
                    .AddLight(new Light(1, 9) {Color = LightColor.Red, SwitchState = SwitchState.On})
                    ;

                masterModel.AddUnassignedLight(new Light(3333, 3));

                var serializer = container.GetInstance<IJsonSerializerService>();
                var json = serializer.SerializeMasterModel(masterModel);

                Console.WriteLine(json);

                var deserialized = serializer.DeserializeMasterModel(json);

                var comparer =
                    new CompareLogic(new ComparisonConfig
                    {
                        MembersToIgnore = new List<string> {"SwitchState", "StatusMode"}
                    });
                var result = comparer.Compare(masterModel, deserialized);

                if (!result.AreEqual)
                {
                    Console.WriteLine("Comparison failed!:");
                    Console.WriteLine("\t{0}", result.DifferencesString);
                    result.AreEqual.ShouldBeTrue();
                }
            }
        }
    }
}