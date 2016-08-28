/// <binding />
"use strict";

var gulp = require("gulp"),
    cleanCSS = require('gulp-clean-css'),
    minifyJS = require('gulp-minify');

var paths = {
  webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";

gulp.task("min:js", function () {
  return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
    .pipe(minifyJS())
    .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
  return gulp.src([paths.css, "!" + paths.minCss])
    .pipe(cleanCSS())
    .pipe(gulp.dest(paths.minCss));
});

gulp.task("min", ["min:js", "min:css"]);