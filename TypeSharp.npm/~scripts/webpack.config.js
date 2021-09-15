const fs = require("fs");
const path = require("path");

var appFolder = "./apps";
var srcFiles = [
    /* If you want to build any apps,
     * type the file names without extension here.
     * (The file name which is start with ~ will be ignored.)
     */
];

function getEntry(dir, filter, recursive) {
    var entries = {};
    function search(_dir) {
        var files = fs.readdirSync(_dir);
        files.forEach((v, i) => {
            if (v.startsWith("~")) return;

            var targetPath = path.join(_dir, v);
            var stat = fs.statSync(targetPath);

            if (stat.isDirectory() && recursive) search(targetPath);
            if (stat.isFile() && filter.test(targetPath)) {
                var match = filter.exec(targetPath);
                var name = match[1] !== undefined ? match[1] : match[0];

                if (srcFiles.length > 0) {
                    if (srcFiles.indexOf(name) > -1) {
                        entries[name] = `./${targetPath}`;
                    }
                }
                else entries[name] = `./${targetPath}`;
            }
        });
    }
    search(dir);
    return entries;
}

module.exports = {
    entry: getEntry(appFolder, /^(?:.+\\)*(.+)\.tsx?$/, true),
    output: {
        filename: "[name].js",
        path: path.join(__dirname, "../wwwroot/js/webpack/")
    },

    // Enable sourcemaps for debugging webpack's output.
    devtool: "source-map",

    resolve: {
        // Add '.ts' and '.tsx' as resolvable extensions.
        extensions: [".ts", ".tsx", ".js", ".json"]
    },

    module: {
        rules: [
            // All files with a '.ts' or '.tsx' extension will be handled by 'awesome-typescript-loader'.
            { test: /\.tsx?$/, loader: "awesome-typescript-loader" },

            // All output '.js' files will have any sourcemaps re-processed by 'source-map-loader'.
            { enforce: "pre", test: /\.js$/, loader: "source-map-loader" }
        ]
    },

    // When importing a module whose path matches one of the following, just
    // assume a corresponding global variable exists and use that instead.
    // This is important because it allows us to avoid bundling all of our
    // dependencies, which allows browsers to cache those libraries between builds.
    externals: {
        "react": "React",
        "react-dom": "ReactDOM",
        "axios": "axios"
    }
};
