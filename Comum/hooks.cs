using System;
using AventStack.ExtentReports;
using TechTalk.SpecFlow;

namespace Ccharp_API_Automation.Comum {

    [Binding]
    class hooks : extendReport {

        private static ExtentTest _scenarioNode;

        [BeforeTestRun]
        public static void BeforeTestRun() {
            ExtentReportInit();
        }

        [Before]
        public static void BeforeScenario(ScenarioContext scenarioContext) {
            var featureName = scenarioContext.ScenarioInfo.Title;

            _feature = _extentReports.CreateTest(featureName);
            _scenarioNode = _feature.CreateNode(scenarioContext.ScenarioInfo.Title);
        }

        [After]
        public static void AfterScenario(ScenarioContext scenarioContext) {
            try {
                if (scenarioContext.TestError != null) {
                    var errorMessage = scenarioContext.TestError.Message;
                    var stackTrace = scenarioContext.TestError.StackTrace ?? "Stack trace not available.";

                    _scenarioNode.Fail($"Test failed with error: {errorMessage}")
                                 .Fail($"Stack trace: {stackTrace}");
                } else {
                    _scenarioNode.Pass("Test Passed");
                }
            } catch (Exception e) {
                Console.WriteLine($"Error in AfterScenario: {e.Message}");
            }
        }

        [AfterStep]
        public static void AfterStep(ScenarioContext scenarioContext) {
            var stepName = scenarioContext.StepContext.StepInfo.Text;
            var stepType = DetermineStepType(stepName);

            switch (stepType) {
                case "Given":
                case "When":
                case "Then":
                    _scenarioNode.Info(stepName).Pass("Success");
                    break;
                default:
                    _scenarioNode.Info(stepName).Pass("Success");
                    break;
            }
        }

        private static string DetermineStepType(string stepName) {
            if (stepName.StartsWith("Given", StringComparison.OrdinalIgnoreCase)) {
                return "Given";
            } else if (stepName.StartsWith("When", StringComparison.OrdinalIgnoreCase)) {
                return "When";
            } else if (stepName.StartsWith("Then", StringComparison.OrdinalIgnoreCase)) {
                return "Then";
            }
            return "And";
        }

        [AfterTestRun]
        public static void AfterTestRun() {
            ExtentReportTearDown();
        }
    }
}
