/// <binding AfterBuild='min' Clean='min' ProjectOpened='min' />
"use strict";

var gulp = require("gulp"),
    rename = require('gulp-rename'),
    uglifyJS = require('gulp-uglify'),
    cleanCSS = require('gulp-clean-css'),
    autoprefixer = require('gulp-autoprefixer');

var paths = {
  webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "min/js";
paths.jsNot = paths.minJs + "/**/*-min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "min/css";
paths.cssNot = paths.minCss + "/**/*-min.css";

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.jsNot])
        .pipe(uglifyJS())
        .pipe(rename({ suffix: "-min" }))
        .pipe(gulp.dest(paths.minJs));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.cssNot])
        .pipe(autoprefixer({  browsers: ['last 2 versions'], cascade: false }))
        .pipe(cleanCSS())
        .pipe(rename({ suffix: "-min" }))
        .pipe(gulp.dest(paths.minCss));
});

gulp.task("min", ["min:js", "min:css"]);