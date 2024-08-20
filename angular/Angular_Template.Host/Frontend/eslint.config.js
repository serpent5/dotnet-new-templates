// @ts-check
const globals = require("globals");
const eslint = require("@eslint/js");
const tseslint = require("typescript-eslint");
const angular = require("angular-eslint");
const eslintConfigPrettier = require("eslint-config-prettier");

module.exports = tseslint.config(
  {
    files: ["**/*.js"],
    extends: [eslint.configs.recommended, eslintConfigPrettier],
    languageOptions: {
      ecmaVersion: "latest",
      globals: globals.node
    }
  },
  {
    files: ["**/*.ts"],
    extends: [
      eslint.configs.recommended,
      ...tseslint.configs.strict,
      ...tseslint.configs.stylistic,
      ...angular.configs.tsRecommended,
      eslintConfigPrettier
    ],
    languageOptions: {
      parserOptions: {
        projectService: true
      }
    },
    processor: angular.processInlineTemplates,
    rules: {
      "@typescript-eslint/explicit-function-return-type": "warn",
      "@typescript-eslint/no-extraneous-class": [
        "warn",
        { allowWithDecorator: true }
      ],
      "@typescript-eslint/no-unused-expressions": "off",
      "@angular-eslint/directive-selector": [
        "error",
        {
          type: "attribute",
          prefix: "app",
          style: "camelCase"
        }
      ],
      "@angular-eslint/component-selector": [
        "error",
        {
          type: "element",
          prefix: "app",
          style: "kebab-case"
        }
      ]
    }
  },
  {
    files: ["**/*.html"],
    extends: [
      ...angular.configs.templateRecommended,
      ...angular.configs.templateAccessibility,
      eslintConfigPrettier
    ],
    rules: {
      "@angular-eslint/template/prefer-self-closing-tags": "error"
    }
  }
);
