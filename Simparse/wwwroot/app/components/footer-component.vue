<template>
    <div>
        <div class="fixed-bottom">
            <nav class="navbar justify-content-center" style="background-color: #f8f9fa !important">
                <a style="font-size:15px;cursor:pointer;" class="navbar-brand" data-toggle="modal" data-target="#contactUsModal"> <b>Contact Us</b></a>
                <a style="font-size:15px;color:black; padding-left:20px;" class="navbar-brand"><i class="far fa-copyright"></i> Simparse Inc</a>
            </nav>
        </div>
        <div id="contactUsModal" class="modal fade" tabindex="-1" role="dialog">
            <div class="modal-dialog modal-xl">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Contact Us</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    </div>
                    <div class="modal-body">
                        <div class="row">
                            <div class="col">
                                <div class="form-group">
                                    <label>Contact Info</label>
                                    <textarea v-model="contactInfo"  type="text" class="form-control" placeholder="Contact Info"></textarea>
                                </div>
                                <div class="form-group">
                                    <label>Subject</label>
                                    <input v-model="subject"  type="text" class="form-control" placeholder="Subject" />
                                </div>
                                <div class="form-group">
                                    <label>Message</label>
                                    <textarea style="height:300px;" v-model="bodyContent" type="text" class="form-control" placeholder="Message"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        <button type="button" class="btn btn-primary" @click="saveClick">Send</button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>

    $.postJSONT = function (url, data, callback) {
        return jQuery.ajax({
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            'type': 'POST',
            'url': url,
            'data': JSON.stringify(data),
            'dataType': 'json',
            'success': callback()
        });
    };

    export default {
        data() {
            return {
                contactInfo: "",
                subject: "",
                bodyContent: ""
            };
        },
        methods: {
            saveClick() {

                var request = {
                    subject: this.subject,
                    textBody: this.bodyContent,
                    contactInfo: this.contactInfo
                };

                $.postJSONT("api/Contact/PostContact", request, function () {
                    toastr.success("Success");
                    $("#contactUsModal").modal("toggle");
                });
            }
        }
    }
</script>