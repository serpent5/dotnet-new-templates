const path = require("path");

module.exports = function (c) {
  c.set({
    ...configOptionsDefaults,
    ...(isCI() ? configOptionsCI : configOptionsDevelopment)
  });
};

const configOptionsDefaults = {
  browsers: ["Chrome"],
  client: {
    clearContext: false
  },
  coverageReporter: {
    dir: path.join(__dirname, "coverage"),
    reporters: [{ type: "text-summary" }]
  },
  frameworks: ["jasmine", "@angular-devkit/build-angular"],
  jasmineHtmlReporter: {
    suppressAll: true
  },
  plugins: [
    require("karma-jasmine"),
    require("karma-chrome-launcher"),
    require("karma-coverage"),
    require("@angular-devkit/build-angular/plugins/karma")
  ],
  reporters: ["coverage"],
  restartOnFileChange: true
};

const configOptionsCI = {
  browsers: ["ChromeHeadless"],
  coverageReporter: {
    ...configOptionsDefaults.coverageReporter,
    dir: path.join(__dirname, "reports/coverage/app"),
    reporters: [
      ...configOptionsDefaults.coverageReporter.reporters,
      { type: "cobertura" }
    ]
  },
  junitReporter: {
    outputDir: path.join(__dirname, "reports/junit/app")
  },
  plugins: [...configOptionsDefaults.plugins, require("karma-junit-reporter")],
  reporters: [...configOptionsDefaults.reporters, "junit"],
  singleRun: true
};

const configOptionsDevelopment = {
  coverageReporter: {
    ...configOptionsDefaults.coverageReporter,
    reporters: [
      ...configOptionsDefaults.coverageReporter.reporters,
      { type: "html" }
    ]
  },
  plugins: [
    ...configOptionsDefaults.plugins,
    require("karma-jasmine-html-reporter")
  ],
  reporters: [...configOptionsDefaults.reporters, "progress", "kjhtml"]
};

function isCI() {
  return process.env["PIPELINE_WORKSPACE"] != null; // Azure DevOps?
}
