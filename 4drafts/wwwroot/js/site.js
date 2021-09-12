// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

//$(document).ready(function () {
//    $('#filter').attr('asp-route-genre', getSelectedValue('genre-select'));
//    $('#filter').attr('asp-route-sort', getSelectedValue('sort-select'));
//});

//function paging() {
//    $('#threadTable').DataTable({
//        "scrollCollapse": true,
//        "paging": true
//    });
//}

$(function () {
    $("#loaderbody").addClass('hide');

    $(document).bind('ajaxStart', function () {
        $("#loaderbody").removeClass('hide');
    }).bind('ajaxStop', function () {
        $("#loaderbody").addClass('hide');
    });
});

var comment_max = 500;
$('#comment_count').html('0 / ' + comment_max);



function countCharacters(counterId, valueId) {
    var text_length = $(document.getElementById(valueId)).val().length;

    var counter = document.getElementById(counterId);
    $(counter).html(text_length + ' / ' + comment_max);
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
        commentButton.classList.remove('disabled');
    } else {
        commentButton.classList.add('disabled');
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

function comment(Id, content) {
    $.ajax({
        url: "/Comments/Create/",
        type: "post",
        data: { threadId: Id, content: content },
        success: function (res) {
            if (!res.isValid) {
                $('.notifyjs-corner').empty();
                $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
            }
            else {
                $('#comment-section').html(res.html);
            }
        }
    });
}

function threadPaging(path, pageNumber) {
    $.ajax({
        url: path,
        type: "get",
        data: { pageNumber: pageNumber },
        success: function (res) {
            $('#threads-section').html(res);
        }
    })
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
                $('.selectpicker').selectpicker();
            },
        error: function (err) {
                console.log(err);
            }
        })
    } catch (e) {
        console.log(e);
    }
}

function refer(id) {
    var form = document.getElementById(id);
    createPost(form);
}

function createPost(form) {
    $.ajax({
        type: 'POST',
        url: form.action,
        data: new FormData(form),
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.isValid) {
                $('#staticBackdrop').modal('hide');
                window.location.href = res.redirectUrl;
            }
            else {
                console.log('here');
                $('#staticBackdrop .modal-body').html(res.html);
                $('.selectpicker').selectpicker();
            }
        },
        error: function (err) {
            console.log(err);
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
                $('.notifyjs-corner').empty();
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
                    $('.notifyjs-corner').empty();
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
                    $('.notifyjs-corner').empty();
                    $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
                }
            }
            else {
                $('.notifyjs-corner').empty();
                $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
            }
        }
    })
}

function editPopup(entityId, path) {
    $.ajax({
        type: 'GET',
        url: path,
        data: { Id: entityId },
        success: function (res) {
            if (res.isValid) {
                $('#staticBackdrop .modal-body').html(res.html);
                $('#staticBackdrop').modal('show');
            }
            else {
                $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
                $('#staticBackdrop').modal('hide');
            }
        }
    })
}

editPost = form => {
    try {
        $.ajax({
            type: 'POST',
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    if (res.entity == 'draft') {
                        $('#drafts-container').html(res.html);
                        $('.notifyjs-corner').empty();
                        $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
                        $('#staticBackdrop').modal('hide');
                    }
                    else if (res.entity == 'thread') {
                        $('#thread-title').text(res.title);
                        $('#thread-content').text(res.content);
                        $('.notifyjs-corner').empty();
                        $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
                        $('#staticBackdrop').modal('hide');
                    }
                    else if (res.entity == 'comment') {
                        $('#comment-section').html(res.html);
                        $('.notifyjs-corner').empty();
                        $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
                        $('#staticBackdrop').modal('hide');
                    }
                }
                else {
                    $('#staticBackdrop .modal-body').html(res.html);
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
            $('#thread-likes-section').html(res.html);
            $('.notifyjs-corner').empty();
            $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
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

function getValue(id) {
    var content = document.getElementById(id);
    var result = $(content).val();
    return result;
}

function getSelectedValue(id) {
    var elem = document.getElementById(id);
    var selected = $(elem).children("option:selected").val();
    return selected;
}

function addFilterRoutes() {
    $('#filter').attr('href', '/Threads/Browse?genre=' + getSelectedValue('genre-select') + '&sort=' + getSelectedValue('sort-select'));
}

function saveDraft(title, description, content, draftId) {
    try {
        $.ajax({
            type: 'POST',
            url: '/Drafts/Save/',
            data: { title: title, description: description, content: content, draftId: draftId },
            success: function (res) {
                if (!res.isValid) {
                    $('.notifyjs-corner').empty();
                    $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
                }
                else {
                    $('.notifyjs-corner').empty();
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

function sort(threadId, type) {
    $.ajax({
        type: 'get',
        url: "/Comments/Sort/",
        data: { threadId: threadId, type: type },
        success: function (res) {
            $('#comment-section').html(res);
        }
    })
}

function cancelModal() {
    $('#form-modal').modal('hide');
    $('#staticBackdrop').modal('hide');
}
