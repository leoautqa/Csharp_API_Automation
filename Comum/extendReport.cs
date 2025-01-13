using System;
using System.IO;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace Ccharp_API_Automation.Comum {
    public class extendReport {
        public static ExtentReports _extentReports;
        public static ExtentTest _feature;
        public static ExtentTest _scenario;

        private static readonly string BaseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\"));
        private static readonly string TestResultPath = Path.Combine(BaseDir, "TestResults");

        private static readonly string ApplicationName = "serverest";

        public static void ExtentReportInit() {
            try {
                EnsureTestResultDirectoryExists();

                var htmlReporter = new ExtentSparkReporter(Path.Combine(TestResultPath, "AutomationStatusReport.html"));
                htmlReporter.Config.ReportName = "Automation Status Report";
                htmlReporter.Config.DocumentTitle = "Automation Status Report";

                _extentReports = new ExtentReports();
                _extentReports.AttachReporter(htmlReporter);
                AddSystemInfo();
            } catch (Exception ex) {
                Console.WriteLine("Erro ao inicializar o relatório: " + ex.Message);
            }
        }

        private static void EnsureTestResultDirectoryExists() {
            if (!Directory.Exists(TestResultPath)) {
                Directory.CreateDirectory(TestResultPath);
            }
        }

        private static void AddSystemInfo() {
            _extentReports.AddSystemInfo("Application", ApplicationName);
        }

        public static void ExtentReportTearDown() {
            try {
                _extentReports?.Flush();
            } catch (Exception ex) {
                Console.WriteLine("Erro ao finalizar o relatório: " + ex.Message);
            }
        }
    }
}
