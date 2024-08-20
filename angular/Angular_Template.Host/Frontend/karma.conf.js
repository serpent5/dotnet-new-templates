const path = require("path");

module.exports = function (c) {
  const reportsPath = path.join(__dirname, "reports");

  const configOptions = {
    client: {
      clearContext: false,
      jasmine: {
        failSpecWithNoExpectations: true
      }
    },
    coverageReporter: {
      dir: path.join(reportsPath, "tests-coverage"),
      subdir: x => x.replace(/ /g, "_")
    },
    frameworks: ["jasmine", "@angular-devkit/build-angular"],
    plugins: [
      require("karma-jasmine"),
      require("karma-chrome-launcher"),
      require("karma-coverage"),
      require("@angular-devkit/build-angular/plugins/karma")
    ]
  };

  const configOptionsInteractive = {
    browsers: ["Chrome"],
    coverageReporter: {
      ...configOptions.coverageReporter,
      reporters: [{ type: "text-summary" }, { type: "html" }]
    },
    mochaReporter: {
      output: "minimal"
    },
    jasmineHtmlReporter: {
      suppressAll: true,
      suppressFailed: true
    },
    plugins: [
      ...configOptions.plugins,
      require("karma-jasmine-html-reporter"),
      require("karma-mocha-reporter")
    ],
    reporters: ["mocha", "kjhtml"],
    restartOnFileChange: true
  };

  const configOptionsHeadless = {
    browsers: [
      process.env.DOCKER_BUILD ? "ChromeHeadlessNoSandbox" : "ChromeHeadless"
    ],
    coverageReporter: {
      ...configOptions.coverageReporter,
      reporters: [{ type: "cobertura" }]
    },
    customLaunchers: {
      ChromeHeadlessNoSandbox: {
        base: "ChromeHeadless",
        flags: ["--no-sandbox"]
      }
    },
    junitReporter: {
      outputDir: path.join(reportsPath, "tests")
    },
    plugins: [...configOptions.plugins, require("karma-junit-reporter")],
    reporters: ["junit"]
  };

  c.set({
    ...configOptions,
    ...(!c.singleRun ? configOptionsInteractive : configOptionsHeadless)
  });
};
