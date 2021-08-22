// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

$(document).ready(function () {
    paging();
});

function paging() {
    $('#threadTable').DataTable({
        "scrollCollapse": true,
        "paging": true
    });
}

$(function () {
    $("#loaderbody").addClass('hide');

    $(document).bind('ajaxStart', function () {
        $("#loaderbody").removeClass('hide');
    }).bind('ajaxStop', function () {
        $("#loaderbody").addClass('hide');
    });
});

var text_max = 500;
$('#count_message').html('0 / ' + text_max);

function countCharacters() {
    var text_length = $('#comment-content').val().length;

    $('#count_message').html(text_length + ' / ' + text_max);
};

function changeArrow() {
    var foldArrow = document.getElementById('fold-arrow');
    var contentBody = document.getElementById('content-body');
    if (contentBody.classList.contains('mh-600')) {
        foldArrow.classList.remove('fa-arrow-down');
        foldArrow.classList.add('fa-arrow-up');
        contentBody.classList.remove('mh-600');
    } else {
        foldArrow.classList.remove('fa-arrow-up');
        foldArrow.classList.add('fa-arrow-down');
        contentBody.classList.add('mh-600');
    }
}

function isEmpty(str) {
    return !$.trim(str).length;
}

function checkIfEmpty() {
    var commentButton = document.getElementById('comment-button');
    var commentContent = document.getElementById('comment-content');

    if (!commentContent.value.length == 0 && /\S/.test(commentContent.value)) {
        $(commentButton).attr('disabled', false);
    } else {
        $(commentButton).attr('disabled', true);
    }
};

function hideButtons() {
    var commentButtons = document.getElementById('comment-buttons');
    var counter = document.getElementById('counter-append');
    commentButtons.classList.add('d-none');
    counter.classList.add('d-none');
}

function showButtons() {
    var commentButtons = document.getElementById('comment-buttons');
    var counter = document.getElementById('counter-append');
    commentButtons.classList.remove('d-none');
    counter.classList.remove('d-none');
}

function getCommentValue() {
    var content = document.getElementById('comment-content');
    var result = $(content).val();
    return result;
}

function refreshComments(Id, Content) {
    $.ajax({
        url: "/Comments/Create/",
        type: "post",
        data: { threadId: Id, Content: Content },
        success: function (result) {
            $('#comment-section').html(result);
        }
    });
}

function createThreadInPopup(title, description, content) {
    try {
        $.ajax({
            type: 'GET',
            url: "/Threads/Create/",
            data: { title: title, description: description, content: content },
        success: function (res) {
                $('#staticBackdrop .modal-body').html(res);
                $('#staticBackdrop').modal('show');
            },
        error: function (err) {
                console.log(err);
            }
        })
    } catch (e) {
        console.log(e);
    }
}

createThreadPost = form => {
    $.ajax({
        type: 'POST',
        url: form.action,
        data: new FormData(form),
        contentType: false,
        processData: false,
        success: function (res) {
            if (!res.isValid) {
                $('#staticBackdrop .modal-body').html(res.html);
            }
            else {
                $('#staticBackdrop').modal('hide');
                window.location.href = res.redirectToUrl;
            }
        }
    })
}

function deletePopup(entityId, path) {
    $.ajax({
        type: 'GET',
        url: path,
        data: { Id: entityId },
        success: function (res) {
            if (res.isValid) {
                $('#form-modal .modal-body').html(res.html);
                $('#form-modal').modal('show');
            }
            else {
                $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
                $('#form-modal').modal('hide');
            }
        }
    })
}

function deletePost(entityId, path) {
    $.ajax({
        type: 'POST',
        url: path,
        data: { Id: entityId },
        success: function (res) {
            if (res.isValid) {
                if (res.entity == 'draft') {
                    document.getElementById(entityId).remove();
                    $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
                    $('#draft-count').html('[' + res.count + ' / 10]');
                    $('#form-modal').modal('hide');
                }
                else if (res.entity == 'thread') {
                    $('#read-container').html("");
                    $('#form-modal').modal('hide');
                    $('#staticBackdrop').modal('show');
                    $('#staticBackdrop .modal-body').html(res.html);
                }
                else if (res.entity == 'comment') {
                    $('#comment-section').html(res.html);
                    $('#form-modal').modal('hide');
                    $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
                }
            }
            else {
                $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
            }
        }
    })
}

function editThreadInPopup(threadId) {
    $.ajax({
        type: 'GET',
        url: "/Threads/Edit/",
        data: { threadId: threadId },
        success: function (res) {
            $('#staticBackdrop .modal-body').html(res);
            $('#staticBackdrop').modal('show');
        },
        error: function (err) {
            console.log(err);
        }
    })
}

editThreadPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (!res.isValid) {
                    $('#staticBackdrop .modal-body').html(res.html);
                }
                else {
                    window.location.href = res.redirectToUrl;
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

function editCommentInPopup(commentId) {
    $.ajax({
        type: 'GET',
        url: "/Comments/Edit/",
        data: { commentId: commentId },
        success: function (res) {
            $('#staticBackdrop .modal-body').html(res);
            $('#staticBackdrop').modal('show');
        },
        error: function (err) {
            console.log(err);
        }
    })
}

editCommentPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (!res.isValid) {
                    $('#staticBackdrop .modal-body').html(res.html);
                }
                else {
                    $('#staticBackdrop').modal('hide');
                    $('#comment-section').html(res.html);
                    $.notify('The comment has been successfully editted', { globalPosition: 'top left', className: 'success' });
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

function likeThread(threadId) {
    $.ajax({
        type: 'Post',
        url: "/Threads/Like/",
        data: { threadId: threadId },
        success: function (res) {
            $('#thread-likes-section').html(res);
        }
    })
}

function likeComment(commentId) {
    $.ajax({
        type: 'post',
        url: "/Comments/Like/",
        data: { commentId: commentId },
        success: function (res) {
            $('#comment-section').html(res);
        }
    })
}

function viewProfile(userId) {
    $.ajax({
        type: 'get',
        url: "/Users/Profile/",
        data: { userId: userId },
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal').modal('show');
        },
        error: function (err) {
            console.log(err);
        }
    })
}

function editAccountInPopup() {
    $.ajax({
        type: 'GET',
        url: "/Users/Edit/",
        success: function (res) {
            $('#staticBackdrop .modal-body').html(res);
            $('#staticBackdrop').modal('show');
        },
        error: function (err) {
            console.log(err);
        }
    })
}

editAccountPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (!res.isValid) {
                    $('#staticBackdrop .modal-body').html(res.html);
                }
                else {
                    window.location.href = res.redirectToUrl;
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

function authGet(path, returnUrl) {
    $.ajax({
        type: 'GET',
        url: path,
        data: { returnUrl: returnUrl },
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal').modal('show');
        },
        error: function (err) {
            console.log(err);
        }
    })
}

authPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (!res.isValid) {
                    $('#form-modal .modal-body').html(res.html);
                }
                else {
                    window.location.href = res.redirectUrl;
                    $('#form-modal').modal('hide');
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

function forgotPasswordPopup() {
    $.ajax({
        type: 'GET',
        url: "/Identity/Account/ForgotPassword/",
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal').modal('show');
        },
        error: function (err) {
            console.log(err);
        }
    })
}

function getFormData() {
    var formData = new FormData(document.getElementById('form-data'));
    return formData;
}

function getValue(id) {
    var content = document.getElementById(id);
    var result = $(content).val();
    return result;
}

function saveDraft(title, description, content, draftId) {
    try {
        $.ajax({
            type: 'POST',
            url: '/Drafts/Save/',
            data: { title: title, description: description, content: content, draftId: draftId },
            success: function (res) {
                if (!res.isValid) {
                    $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
                }
                else {
                    $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
                }
            },
            error: function (err) {
                console.log(err)
            }
        })
        //to prevent default form submit event
        return false;
    } catch (ex) {
        console.log(ex)
    }
}

function cancelModal() {
    $('#form-modal').modal('hide');
    $('#staticBackdrop').modal('hide');
}
