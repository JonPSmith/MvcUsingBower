module.exports = function (grunt) {

    // Project configuration.
    grunt.initConfig({
        //WARNING: You CANNOT put comments in the setting.json file. grunt.file.readJSON does not handle comments
        properties: grunt.file.readJSON('App_Data/BowerBundles.json'),
        concat: {
            //This is the standard way of concatenating files using setting.json
            // - scr:  takes an array by accessing a json property, in this case mainCss, from the setting.json file
            // - dest: to use BunndleForBower.cs you need to save the file into a set directory and with the same name as the property
            css: {
                src: new Array('<%= properties.mainCss %>'),
                dest: 'css/mainCss.css'
            },
            standardLibsJs: {
                src: new Array('<%= properties.standardLibsJs %>'),
                dest: 'js/standardLibsJs.js'
            },
            appLibsJs: {
                src: new Array('<%= properties.appLibsJs %>'),
                dest: 'js/appLibsJs.js'
            },
            jqueryval: {
                src: new Array('<%= properties.jqueryval %>'),
                dest: 'js/jqueryval.js'
            }
        },

        cssmin: {
            css: {
                src: 'css/mainCss.css',
                dest: 'css/mainCss.min.css'
            }
        },

        uglify: {
            standardLibsJs: {
                src: 'js/standardLibsJs.js',
                dest: 'js/standardLibsJs.min.js'
            },
            appLibsJs: {
                src: 'js/appLibsJs.js',
                dest: 'js/appLibsJs.min.js'
            },
            jqueryval: {
                src: 'js/jqueryval.js',
                dest: 'js/jqueryval.min.js'
            }
        },
        //This is only needed when Bootstrap and/or Jquery is updated
        copy: {
            jQueryCdn: {
                src: 'lib/jquery/dist/jquery.min.js',
                dest: 'js/jquery.min.js'
            },
            bootstrapCdn: {
                src: 'lib/bootstrap/dist/js/bootstrap.min.js',
                dest: 'js/bootstrap.min.js'
            },
            fonts: {
                expand: true,
                cwd: 'lib/bootstrap/fonts/',
                src: '*',
                flatten: true,
                dest: 'fonts/'
            }
        },

        watch: {
            configFiles: {
                files: ['gruntfile.js'],
                options: {
                    reload: true
                }
            },
            //These watch various libraray and specific application directories for changes and then call the appropriate build commands
            css: {
                files: ['lib/**/*.css', 'Content/**/*.css'],
                tasks: ['build:css'],
                options: {
                    spawn: false,
                },
            },
            js: {
                files: ['lib/**/*.js', 'Script/**/*.js'],
                tasks: ['build:js'],
                options: {
                    spawn: false,
                },
            }
        }
    });

    // Load the various plugin that we need
    grunt.loadNpmTasks('grunt-contrib-concat');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-watch');
    grunt.loadNpmTasks('grunt-contrib-copy');

    // grunt.registerTask allows you to define 'macro' commands that call multiple commands
    grunt.registerTask('default', []);

    // Build task(s).
    grunt.registerTask('build:css', ['concat:css', 'cssmin:css']);

    // Build task(s).
    grunt.registerTask('build:js', [
        'concat:standardLibsJs', 'concat:appLibsJs', 'concat:jqueryval',
        'uglify:standardLibsJs', 'uglify:appLibsJs', 'uglify:jqueryval']);

    // Build task(s).
    grunt.registerTask('build', ['build:css', 'build:js']);

};