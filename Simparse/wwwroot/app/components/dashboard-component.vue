
<template>
    <div>
        <nav class="navbar navbar-light bg-light">
            <a style="font-size:30px;" class="navbar-brand"> &nbsp;&nbsp;&nbsp;Simparse</a>
            <div class="form-inline ml-auto">
                <div v-if="fileSystemIsVisibile || canvasIsVisible">
                    <div class="navbar-r" style="padding-right:9px;padding-left: 5px;">
                        <button title="Profile" @click="showProfile" class="btn btn-lg btn-outline-primary"><i class="fas fa-user" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>My Profile</b></button>
                    </div>
                </div>
                <div v-if="profileIsVisible">
                    <div class="navbar-r" style="padding-right:9px;padding-left: 5px;">
                        <button title="File System" @click="showManager" class="btn btn-lg btn-outline-primary"><i class="fas fa-folder" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>File Manager</b></button>
                    </div>
                </div>
                <div class="navbar-r" style="padding-left:5px;">
                    <button title="Logout" @click="logout" class="btn btn-lg btn-outline-danger"><i class="fas fa-sign-out" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>Sign out</b></button>
                </div>
            </div>
        </nav>
        <div class="container-fluid" style="padding:75px;">
            <div v-if="fileSystemIsVisibile">
                <div v-if="isDirectoryShowing">
                    <!--Folder ToolBar-->
                    <div style="border: 1px solid #cecece;">
                        <nav class="navbar navbar-light bg-light">
                            <div class="form-inline">
                                <div style="padding-left:5px;">
                                    <button title="Add New Folder" @click="addNewCategory" class="btn btn-lg btn-outline-primary"><i class="fas fa-plus"></i>&nbsp;&nbsp;&nbsp;<b>Add Folder</b></button>
                                </div>
                            </div>
                        </nav>
                    </div>
                </div>
                <div v-if="!isDirectoryShowing">
                    <!--File ToolBar-->
                    <div style="border: 1px solid #cecece;">
                        <nav class="navbar navbar-light bg-light">
                            <div class="form-inline">
                                <div style="padding-left:5px;">
                                    <button title="Back" @click="onBackClick" class="btn btn-lg btn-outline-danger"><i class="fas fa-chevron-square-left" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>Back</b></button>
                                </div>
                                <div style="padding-left:5px;">
                                    <button title="Upload File" @click="onFileUploadClick" class="btn btn-lg btn-outline-primary"><i class="fas fa-file-upload" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>Upload</b></button>
                                    <input @change="onFileUpload" id="hiddenFileUpload" type="file" style="display:none;" />
                                </div>
                            </div>
                            <div class="form-inline ml-auto">
                                <div class="navbar-r" style="padding-left:5px;">
                                    <button title="Excel Export" @click="xlsExport" class="btn btn-lg btn-outline-success"><i class="fas fa-file-excel" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>Excel Export</b></button>
                                </div>
                                <div class="navbar-r" style="padding-right:9px;padding-left: 5px;">
                                    <button title="JSON Export" @click="jsonExport" class="btn btn-lg btn-outline-dark"><i class="fas fa-file-code" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>JSON Export</b></button>
                                </div>
                                <div class="navbar-r" style="padding-left:5px;">
                                    <button title="Upload File" @click="deleteFolder" class="btn btn-lg btn-outline-danger"><i class="fas fa-trash-alt" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>Delete</b></button>
                                </div>
                            </div>
                        </nav>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-12">
                        <div style="border: 1px solid #cecece;min-height:800px;">
                            <div style="padding:20px;">
                                <div class="row">
                                    <tree-folder-item v-for="fileDir in fileDirs" @onFileClick="onFileClick" @onFolderClick="onFolderClick" v-bind:key="fileDir.id" :node="fileDir"></tree-folder-item>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div v-if="profileIsVisible">
                <profile-comp :current-user="currentUser"></profile-comp>
            </div>

            <!--File Viewer Modal-->
            <div v-if="canvasIsVisible">
                <div class="row">
                    <div class="col">
                        <nav class="navbar navbar-light bg-light">
                            <div style="padding-left:5px;padding-right:15px;">
                                <button title="Back" @click="showManager" class="btn btn-lg btn-outline-danger"><i class="fas fa-chevron-square-left" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>Back</b></button>
                            </div>
                            <div class="form-inline">
                                <div style="padding-left:5px;">
                                    <span>Pages</span>
                                </div>
                                <div style="padding-left:5px;">
                                    <button :disabled="isPrevButtonDisabled" title="Previous" @click="prevPageClick" class="btn btn-outline-dark"><i class="fas fa-caret-left" style="font-size:large"></i></button>
                                </div>
                                <div style="padding-left:5px;">
                                    <span>{{pageNumber}} of {{pageCount}}</span>
                                </div>
                                <div style="padding-left:5px;">
                                    <button :disabled="isNextButtonDisabled" title="Next" @click="nextPageClick" class="btn btn-outline-dark"><i class="fas fa-caret-right" style="font-size:large"></i></button>
                                </div>
                                <div style="padding-left:25px;">
                                    <button :disabled="isUndoButtonDisabled" title="Undo" @click="undoClick" class="btn btn-success"><i class="fas fa-undo" style="font-size:large"></i> Undo</button>
                                </div>
                                <div style="padding-left:25px;">
                                    <button title="Undo" @click="exportFileJSON" class="btn btn-primary"><i class="fas fa-file-code" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;JSON Export</button>
                                </div>
                                <div style="padding-left:25px;">
                                    <!--<button title="Plus" @click="zoomPlus" class="btn btn-primary">Plus</button>
                                    <button title="Plus" @click="zoomMinus" class="btn btn-primary">Minus</button>-->
                                </div>
                            </div>
                            <div class="form-inline ml-auto">
                                <div class="navbar-r" style="padding-left:5px;">
                                    <button title="Save" @click="saveFileMappings" class="btn btn-outline-primary"><i class="fas fa-save" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>Save</b></button>
                                    <button title="Upload File" @click="deleteFile" class="btn btn-outline-danger"><i class="fas fa-trash-alt" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>Delete</b></button>
                                </div>
                            </div>
                        </nav>
                    </div>
                </div>
                <br />
                <div class="row">
                    <div class="col-4">
                        <div class="row">
                            <div class="col-1">

                            </div>
                            <div class="col-3">
                                <span>Name</span>
                            </div>
                            <div class="col-3">
                                <span>Text</span>
                            </div>
                            <div class="col-2">
                                <span>Type</span>
                            </div>
                            <div class="col-3">

                            </div>
                        </div>
                        <div v-for="mappingItem in mappingItems">
                            <div class="row" style="padding-top:7px;">
                                <div class="col-1">

                                </div>
                                <div class="col-3">
                                    <input type="text" class="form-control" v-model="mappingItem.name" />
                                </div>
                                <div class="col-3">
                                    <input type="text" class="form-control" v-model="mappingItem.textExtract" />
                                </div>
                                <div class="col-2">
                                    <select class="form-control" v-model="mappingItem.type" @change="onTypeChange($event, mappingItem.id)">
                                        <option value="0">Text</option>
                                        <option value="1">Number</option>
                                    </select>
                                </div>

                                <div class="col-3">
                                    <button title="Delete File" @click="deleteFileMapping(mappingItem.id)" class="btn btn-outline-danger"><i class="fas fa-trash-alt" style="font-size:large"></i>&nbsp;&nbsp;&nbsp;<b>Delete</b></button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-8">
                        <div id="myCanvas"></div>
                    </div>
                </div>
            </div>
        </div>




        <footer-comp></footer-comp>
    </div>

</template>

<script>
    import icon from './icon-component.vue';
    import footer from './footer-component.vue';
    import profile from './profile-component.vue';
    import { ajax } from '../ajax.js';
    let ajaxhelper = new ajax();
    let canvasApp = null;


    export default {
        components: {
            "tree-folder-item": icon,
            "footer-comp": footer,
            "profile-comp": profile
        },
        data() {
            return {
                token: null,
                currentUser: { email: "" },

                fileDirs: [],
                folderId: null,
                fileId: null,
                isDirectoryShowing: true,
                pageNumber: 1,
                zoom: 100,
                pageCount: 0,
                isPreviousPageButtonEnabled: false,
                isNextPageButtonEnabled: false,
                mappingItems: [],
                lines: [],
                words: [],
                profileIsVisible: false,
                fileSystemIsVisibile: true,
                canvasIsVisible: false
            };
        },
        computed: {
            isPrevButtonDisabled() {
                if (this.pageNumber == 1) {
                    return true;
                }
                return false;
            },
            isNextButtonDisabled() {
                if (this.pageNumber == (this.pageCount) || this.pageCount == 1) {
                    return true;
                }
                return false;
            },
            isUndoButtonDisabled() {
                if (this.lines.length > 0) {
                    return false;
                }
                return true;
            }
        },
        methods: {

            logout() {
                Cookies.remove("authInfo");
                document.location.href = "/";
            },
            xlsExport() {
                window.location = "/api/file/GetFolderXLS/" + this.folderId;
            },
            jsonExport() {
                window.location = "/api/file/GetFolderJSON/" + this.folderId;
            },
            exportFileJSON() {
                window.location = "/api/file/GetFileJSON/" + this.fileId;
            },
            undoClick() {
                this.lines.splice(this.lines.length - 1, 1);
                this.refreshCanvas();
            },
            onTypeChange(event, id) {
                var value = parseInt(event.target.value);
                for (var i = 0; i < this.mappingItems.length; i++) {
                    if (this.mappingItems[i].id == id) {
                        this.mappingItems[i].type = value;
                    }
                }
            },
            addNewCategory() {
                var self = this;
                bootbox.prompt("Create Folder Name", function (result) {
                    if (result) {
                        var request = { name: result };
                        ajaxhelper.post(request, "/api/file/CreateFolder", self.token).then(function (response) {
                            self.showFolders();
                            toastr.success("Success");
                        }).catch(function (e) {
                            toastr.error("Failure");
                            console.log(e);
                        });
                    }
                });
            },
            onFileUpload(e) {
                var self = this;
                var request = { FolderId: this.folderId };
                var formData = new FormData();
                formData.append("file", e.target.files[0]);
                bootbox.dialog({
                    message: '<div class="text-center"><i class="fa fa-spin fa-spinner"></i> Loading...</div>',
                    closeButton: false
                });
                ajaxhelper.upload(formData, JSON.stringify(request), this.token).then(function (response) {
                    bootbox.hideAll();
                    self.showFiles();
                    toastr.success("Success");
                }).catch(function (e) {
                    toastr.error("Failure");
                    console.log(e);
                });
            },
            onFileUploadClick(e) {
                $("#hiddenFileUpload").click();
            },
            showProfile() {
                this.profileIsVisible = true;
                this.fileSystemIsVisibile = false;
                this.canvasIsVisible = false;
            },
            showManager() {
                this.profileIsVisible = false;
                this.fileSystemIsVisibile = true;
                this.canvasIsVisible = false;
            },
            showCanvas() {
                this.canvasIsVisible = true;
                this.fileSystemIsVisibile = false;
            },
            deleteFile() {
                var self = this;
                bootbox.confirm("Are you sure you want to delete this file?", function (result) {
                    if (result) {
                        ajaxhelper.get("/api/file/DeleteFile/" + self.fileId, self.token).then(function (response) {
                            self.showFiles();
                            toastr.success("Success");
                        }).catch(function (e) {
                            toastr.error("Failure");
                            console.log(e);
                        });
                    }
                    self.showManager();
                });
            },
            deleteFolder() {
                var self = this;
                bootbox.confirm("Are you sure you want to delete this folder and all of its content?", function (result) {
                    if (result) {
                        ajaxhelper.get("/api/file/DeleteFolder/" + self.folderId, self.token).then(function (response) {
                            self.folderId = null;
                            self.isDirectoryShowing = true;
                            self.showFolders();
                            toastr.success("Success");
                        }).catch(function (e) {
                            toastr.error("Failure");
                            console.log(e);
                        });
                    }
                });
            },
            onBackClick() {
                this.folderId = null;
                this.showFolders();
                this.isDirectoryShowing = true;
            },
            showFiles() {
                var self = this;
                ajaxhelper.get("/api/file/GetFiles/" + this.folderId, this.token).then(function (children) {
                    self.fileDirs = children;
                }).catch(function (e) {
                    toastr.error("Failure");
                    console.log(e);
                });
            },
            showFolders() {
                var self = this;
                ajaxhelper.get("/api/file/GetFolders/", this.token).then(function (children) {
                    self.fileDirs = children;
                }).catch(function (e) {
                    toastr.error("Failure");
                    console.log(e);
                });
            },
            onFolderClick(id) {
                this.folderId = id;
                this.showFiles();
                this.isDirectoryShowing = false;
            },
            onFileClick(id) {
                var self = this;
                this.fileId = id;
                this.pageNumber = 1;
                this.lines = [];
                this.loadFolderMappings(false);
                self.showCanvas();
                this.loadWordsOnCanvas(function () {

                    self.refreshCanvas();
                });

            },
            zoomPlus() {
                this.zoom += 100;
                this.loadWordsOnCanvas();
            },
            zoomMinus() {
                this.zoom -= 100;
                this.loadWordsOnCanvas();
            },
            clearTheCanvas(index) {
                if (canvasApp && canvasApp.stage.children) {
                    while (canvasApp.stage.children[index]) { canvasApp.stage.removeChild(canvasApp.stage.children[index]); }
                }
            },
            loadWordsOnCanvas(callback) {
                var self = this;
                this.clearTheCanvas(2);
                var url = "/api/file/GetFileDataForCanvas/" + this.fileId + "/" + this.pageNumber + "/" + this.zoom;
                ajaxhelper.get(url, self.token).then(function (response) {
                    if (canvasApp) {
                        $("#myCanvas").empty();
                        self.mountTheCanvas(response.width, response.height);
                    } else {
                        self.mountTheCanvas(response.width, response.height);
                    }

                    self.pageCount = response.pageCount;
                    self.words = response.items;
                    self.bindWordsOnCanvas(response.items);
                    if (callback) {
                        callback();
                    }
                }).catch(function (e) {
                    toastr.error("Failure");
                    console.log(e);
                });
            },
            bindWordsOnCanvas(words) {
                for (var i = 0; i < words.length; i++) {
                    var word = words[i];
                    var wordStyling = new PIXI.TextStyle({
                        fontFamily: word.font,
                        fontSize: word.size + 2,
                        align: "left"
                    });
                    var basicText = new PIXI.Text(word.text, wordStyling);
                    basicText.interactive = false;
                    basicText.x = word.x;
                    basicText.y = word.y;
                    basicText.anchor.set(0.5);
                    canvasApp.stage.addChild(basicText);
                }
            },
            nextPageClick(e) {
                this.pageNumber = (this.pageNumber + 1);
                this.loadFolderMappings(true);
                this.loadWordsOnCanvas();
            },
            prevPageClick(e) {
                this.pageNumber = (this.pageNumber - 1);
                this.loadFolderMappings(true);
                this.loadWordsOnCanvas();
            },
            loadFolderMappings(doRefresh) {
                var self = this;
                ajaxhelper.get("/api/file/GetMappings/" + this.folderId + "/file/" + this.fileId + "/page/" + this.pageNumber).then(function (response) {
                    self.mappingItems = response;
                    if (doRefresh) {
                        self.refreshCanvas();
                    }
                }).catch(function (e) {
                    toastr.error("Failure");
                    console.log(e);
                });
            },
            refreshCanvas() {
                var self = this;
                this.clearTheCanvas(2);
                this.bindWordsOnCanvas(this.words);
                setTimeout(function () {
                    self.bindCurrentLinesOnCanvas();
                    self.bindSavedLinesOnCanvas();
                }, 500);
            },
            bindCurrentLinesOnCanvas() {
                const realPath = new PIXI.Graphics();
                realPath.lineStyle(2, 0x9EC1A4, 1);
                for (var i = 0; i < this.lines.length; i++) {
                    if (i == 0) {
                        realPath.moveTo(this.lines[i].x, this.lines[i].y);
                    } else {
                        realPath.lineTo(this.lines[i].x, this.lines[i].y);
                    }
                }
                canvasApp.stage.addChild(realPath);
            },
            bindSavedLinesOnCanvas() {
                //persisted line binder
                for (var i = 0; i < this.mappingItems.length; i++) {
                    const actualPath = new PIXI.Graphics();
                    actualPath.lineStyle(2, 0x0000ff, 1);
                    for (var x = 0; x < this.mappingItems[i].items.length; x++) {
                        if (x == 0) {
                            actualPath.moveTo(this.mappingItems[i].items[x].x, this.mappingItems[i].items[x].y);
                        } else {
                            actualPath.lineTo(this.mappingItems[i].items[x].x, this.mappingItems[i].items[x].y);
                        }
                    }
                    canvasApp.stage.addChild(actualPath);
                }
            },
            deleteFileMapping(id) {
                var self = this;
                ajaxhelper.get("/api/file/DeleteMapping/" + id).then(function () {
                    self.loadFolderMappings(true);
                    toastr.success("Success");
                }).catch(function (e) {
                    toastr.error("Failure");
                    console.log(e);
                });
            },
            insertMapping(vectorArray) {
                var self = this;
                var count = this.mappingItems.length + 1;
                var request = { items: vectorArray, folderId: this.folderId, pageNumber: this.pageNumber, name: "Mapping#" + count, type: 0 };
                ajaxhelper.post(request, "/api/file/InsertMapping").then(function () {
                    self.loadFolderMappings(true);
                    toastr.success("Success");
                }).catch(function (e) {
                    toastr.error("Failure");
                    console.log(e);
                });
            },
            onCanvasCrosshairClick(e) {
                let xVec = e.data.global.x;
                let yVec = e.data.global.y;
                var newVector = { x: xVec, y: yVec };
                console.log(newVector);
                let didIntersect = this.doesPointIntersect(newVector);
                this.lines.push(newVector);
                if (didIntersect) {
                    this.insertMapping(this.lines);
                    this.lines = [];
                } else {
                    this.refreshCanvas();
                }
            },
            doesPointIntersect(newVector) {
                var lastVector = this.lines[this.lines.length - 1];
                if (this.lines.length > 2) {
                    for (var i = 0; i < this.lines.length - 2; i++) {
                        var current = this.lines[i];
                        var next = this.lines[i + 1];
                        var doesIntersect = intersects(current.x, current.y, next.x, next.y, lastVector.x, lastVector.y, newVector.x, newVector.y);
                        if (doesIntersect) {
                            return true;
                        }
                    }
                }
                return false;
            },
            saveFileMappings() {
                var request = { items: this.mappingItems };
                ajaxhelper.post(request, "/api/file/updateMappings").then(function (response) {
                    toastr.success("Success");
                }).catch(function (e) {
                    toastr.error("Failure");
                    console.log(e);
                });
            },
            mountTheCanvas(width, height) {

                var self = this;
                canvasApp = new PIXI.Application({ backgroundColor: 0xFFFFFF, width: width, height: height, autoresize: false });
                canvasApp.renderer.plugins.interaction.cursorStyles.default = "crosshair";
                //background click listener
                let bg = new PIXI.Sprite(PIXI.Texture.WHITE);
                bg.width = canvasApp.screen.width;
                bg.height = canvasApp.screen.height;
                bg.interactive = true;
                bg.on('pointerdown', function (e) {
                    self.onCanvasCrosshairClick(e);
                });

                const realPath = new PIXI.Graphics();
                realPath.lineStyle(2, 0x9EC1A4, 1);
                realPath.moveTo(0, 0);
                realPath.lineTo(width, 0);
                realPath.lineTo(width, height);
                realPath.lineTo(0, height);
                realPath.lineTo(0, 0);

                canvasApp.stage.addChild(bg);
                canvasApp.stage.addChild(realPath);
                //bind canvas
                document.getElementById("myCanvas").appendChild(canvasApp.view);
            },
            authCheck() {
                var token = Cookies.get('authInfo');
                var self = this;
                if (token) {
                    ajaxhelper.post({ Token: token }, "/api/identity/validatetoken", null).then(function (response) {
                        Cookies.set('authInfo', response.token);
                        self.token = response.token;
                        console.log(response);
                        self.currentUser = response.user;
                        self.showFolders();
                    }).catch(function (e) {
                        toastr.error("Failure");
                        console.log(e);
                        Cookies.remove("authInfo");
                        document.location.href = "/";
                    });
                } else {
                    Cookies.remove("authInfo");
                    document.location.href = "/";
                }
            }
        },
        mounted() {
            this.authCheck();
        }
    };


    function intersects(a, b, c, d, p, q, r, s) {
        var det, gamma, lambda;
        det = (c - a) * (s - q) - (r - p) * (d - b);
        if (det === 0) {
            return false;
        } else {
            lambda = ((s - q) * (r - a) + (p - r) * (s - b)) / det;
            gamma = ((b - d) * (r - a) + (c - a) * (s - b)) / det;
            return (0 < lambda && lambda < 1) && (0 < gamma && gamma < 1);
        }
    };


</script>