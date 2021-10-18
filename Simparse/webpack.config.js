/// <binding BeforeBuild='Run - Development' ProjectOpened='Hot' />
const VueLoaderPlugin = require('./node_modules/vue-loader/lib/plugin')
const path = require('path');

var dashboardBundle = {
    mode: "development",
    entry: "./wwwroot/app/app.js",
    output: {
        filename: "./app/dist/dashboardBundle.js",
        path: path.resolve(__dirname, 'wwwroot')
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            }
        ]
    },
    plugins: [
        new VueLoaderPlugin()
    ],
    resolve: {
        alias: {
            'vue$': 'vue/dist/vue.esm.js'
        }
    }
}


var indexBundle = {
    mode: "development",
    entry: "./wwwroot/app/landing.js",
    output: {
        filename: "./app/dist/indexBundle.js",
        path: path.resolve(__dirname, 'wwwroot')
    },
    module: {
        rules: [
            {
                test: /\.vue$/,
                loader: 'vue-loader'
            }
        ]
    },
    plugins: [
        new VueLoaderPlugin()
    ],
    resolve: {
        alias: {
            'vue$': 'vue/dist/vue.esm.js'
        }
    }
}

module.exports = [indexBundle, dashboardBundle];
