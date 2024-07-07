const path = require("path");

module.exports = function (c, projectName) {
  const reportsPath = path.join(__dirname, "reports");

  const configOptionsDefaults = {
    browsers: ["ChromeHeadless"],
    client: {
      clearContext: false,
      jasmine: {
        failSpecWithNoExpectations: true
      }
    },
    coverageReporter: {
      dir: path.join(reportsPath, "coverage", projectName),
      subdir: ".",
      reporters: [{ type: "text-summary" }]
    },
    frameworks: ["jasmine", "@angular-devkit/build-angular"],
    plugins: [
      require("karma-jasmine"),
      require("karma-chrome-launcher"),
      require("karma-coverage"),
      require("@angular-devkit/build-angular/plugins/karma")
    ],
    reporters: ["coverage"],
    restartOnFileChange: true
  };

  const configOptionsDevelopment = {
    coverageReporter: {
      ...configOptionsDefaults.coverageReporter,
      reporters: [
        ...configOptionsDefaults.coverageReporter.reporters,
        { type: "html" }
      ]
    },
    mochaReporter: {
      output: "minimal"
    },
    plugins: [
      ...configOptionsDefaults.plugins,
      require("karma-mocha-reporter")
    ],
    reporters: [...configOptionsDefaults.reporters, "mocha"]
  };

  const configOptionsCI = {
    coverageReporter: {
      ...configOptionsDefaults.coverageReporter,
      reporters: [
        ...configOptionsDefaults.coverageReporter.reporters,
        { type: "cobertura" }
      ]
    },
    junitReporter: {
      outputDir: path.join(reportsPath, "junit", projectName)
    },
    plugins: [
      ...configOptionsDefaults.plugins,
      require("karma-junit-reporter")
    ],
    reporters: [...configOptionsDefaults.reporters, "junit"],
    singleRun: true
  };

  c.set({
    ...configOptionsDefaults,
    ...(isCI() ? configOptionsCI : configOptionsDevelopment)
  });
};

function isCI() {
  return process.env["PIPELINE_WORKSPACE"] != null; // Azure DevOps?
}
