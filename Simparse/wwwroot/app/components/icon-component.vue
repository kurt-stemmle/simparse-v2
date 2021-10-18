<template>
    <div class="col-xs-1">
        <div style="padding:15px;">
            <div class="folderTreeItemRow" @click="folderClick(node.id, node.isDirectory)">
                <div v-if="node.isDirectory">
                    <div class="row">
                        <div class="col">
                            <i style="font-size:100px;" class="far fa-folder-open"></i>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <span style="font-size:15px;">{{node.name}}</span>
                        </div>
                    </div>
                </div>
                <div v-if="!node.isDirectory">
                    <div class="row">
                        <div v-if="node.fileIconType == 1" class="col">
                            <i style="font-size:100px;" class="far fa-file-image"></i>
                        </div>
                        <div v-if="node.fileIconType == 2" class="col">
                            <i style="font-size:100px;" class="far fa-file-pdf"></i>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col">
                            <span style="font-size:15px;">{{node.name}}</span>
                        </div>
                    </div>
                </div>
            </div>
        </div>

    </div>
</template>

<script>
    export default {
        props: ['node'],
        methods: {
            folderClick: function (id, isDir) {
                if (isDir) {
                    this.$emit("onFolderClick", id);
                } else {
                    this.$emit("onFileClick", id);
                }
            }
        }
    }
</script>