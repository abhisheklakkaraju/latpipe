const gulp = require('gulp');
const rimraf = require('rimraf');
const less = require('gulp-less');
const concat = require('gulp-concat');
const uglifycss = require('gulp-uglifycss');
const uglifyjs = require('gulp-uglify-es').default;

gulp.task('clean.css', (callback) => {
    return rimraf('./wwwroot/css/**/*.min.css', callback);
});

gulp.task('clean.js', (callback) => {
    return rimraf('./wwwroot/js/**/*.min.js', callback);
});

gulp.task('clean', gulp.series([
    'clean.css',
    'clean.js'
]));

gulp.task('less', () => {
    return gulp
        .src('./wwwroot/css/**/*.less')
        .pipe(less())
        .pipe(gulp.dest(file => {
            return file.base;
        }));
});

gulp.task('vendor.private.css', () => {
    return gulp
        .src([
            './wwwroot/css/rome/*.css',
            './wwwroot/css/bootstrap/*.css',
            './wwwroot/css/font-awesome/*.css',
            './wwwroot/css/mvc-grid/*.css',
            './wwwroot/css/mvc-tree/*.css',
            './wwwroot/css/mvc-lookup/*.css'
        ])
        .pipe(concat('./wwwroot/css/private/vendor.min.css'))
        .pipe(uglifycss())
        .pipe(gulp.dest('.'));
});

gulp.task('site.private.css', () => {
    return gulp
        .src([
            './wwwroot/css/shared/alerts.css',
            './wwwroot/css/shared/content.css',
            './wwwroot/css/shared/header.css',
            './wwwroot/css/shared/navigation.css',
            './wwwroot/css/shared/overrides.css',
            './wwwroot/css/shared/table.css',
            './wwwroot/css/shared/widget-box.css',
            './wwwroot/css/shared/private.css'
        ])
        .pipe(concat('./wwwroot/css/private/site.min.css'))
        .pipe(uglifycss())
        .pipe(gulp.dest('.'));
});

gulp.task('vendor.public.css', () => {
    return gulp
        .src([
            './wwwroot/css/bootstrap/*.css',
            './wwwroot/css/font-awesome/*.css'
        ])
        .pipe(concat('./wwwroot/css/public/vendor.min.css'))
        .pipe(uglifycss())
        .pipe(gulp.dest('.'));
});

gulp.task('site.public.css', () => {
    return gulp
        .src([
            './wwwroot/css/shared/alerts.css',
            './wwwroot/css/shared/content.css',
            './wwwroot/css/shared/overrides.css',
            './wwwroot/css/shared/public.css'
        ])
        .pipe(concat('./wwwroot/css/public/site.min.css'))
        .pipe(uglifycss())
        .pipe(gulp.dest('.'));
});

gulp.task('app.css', () => {
    return gulp
        .src([
            './wwwroot/css/application/**/*.css',
            '!./wwwroot/css/application/**/*.min.css'
        ])
        .pipe(uglifycss())
        .pipe(gulp.dest(file => {
            file.basename = file.basename.split('.', 1)[0] + '.min.css';

            return file.base;
        }));
});

gulp.task('vendor.private.js', () => {
    return gulp
        .src([
            './wwwroot/js/numbro/numbro.js',
            './wwwroot/js/numbro/**/*.js',
            './wwwroot/js/moment/moment.js',
            './wwwroot/js/moment/**/*.js',
            './wwwroot/js/rome/*.js',
            './wwwroot/js/mvc-lookup/**/*.js',
            './wwwroot/js/mvc-grid/**/*.js',
            './wwwroot/js/mvc-tree/*.js',
            './wwwroot/js/bootstrap/*.js',
            './wwwroot/js/wellidate/*.js',
            './wwwroot/js/shared/widgets/*.js'
        ])
        .pipe(concat('./wwwroot/js/private/vendor.min.js'))
        .pipe(uglifyjs({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest('.'));
});

gulp.task('site.private.js', () => {
    return gulp
        .src([
            './wwwroot/js/shared/private.js'
        ])
        .pipe(concat('./wwwroot/js/private/site.min.js'))
        .pipe(uglifyjs({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest('.'));
});

gulp.task('vendor.public.js', () => {
    return gulp
        .src([
            './wwwroot/js/bootstrap/*.js',
            './wwwroot/js/wellidate/*.js',
            './wwwroot/js/shared/widgets/validator.js',
            './wwwroot/js/shared/widgets/alerts.js'
        ])
        .pipe(concat('./wwwroot/js/public/vendor.min.js'))
        .pipe(uglifyjs({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest('.'));
});

gulp.task('site.public.js', () => {
    return gulp
        .src([
            './wwwroot/js/shared/public.js'
        ])
        .pipe(concat('./wwwroot/js/public/site.min.js'))
        .pipe(uglifyjs({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest('.'));
});

gulp.task('app.js', () => {
    return gulp
        .src([
            './wwwroot/js/application/**/*.js',
            '!./wwwroot/js/application/**/*.min.js'
        ])
        .pipe(uglifyjs({
            output: {
                comments: /^!/
            }
        }))
        .pipe(gulp.dest(file => {
            file.basename = file.basename.split('.', 1)[0] + '.min.js';

            return file.base;
        }));
});

gulp.task('watch', () => {
    gulp.watch('./wwwroot/css/**/*.less', gulp.series([
        'less'
    ]));
});

gulp.task('minify', gulp.series([
    'clean.css',
    'clean.js',
    'less',
    'vendor.private.css',
    'vendor.public.css',
    'site.private.css',
    'site.public.css',
    'app.css',
    'vendor.private.js',
    'vendor.public.js',
    'site.private.js',
    'site.public.js',
    'app.js'
]));

gulp.task('default', gulp.series([
    'minify'
]));
