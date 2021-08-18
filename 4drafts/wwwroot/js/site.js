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

function showComments() {
    var showCommentsButton = document.getElementById('comments-button');
    var commentSection = document.getElementById('comment-section');

    if (commentSection.classList.contains('d-none')) {
        commentSection.classList.remove('d-none');
        showCommentsButton.value = 'hide comments'
        showCommentsButton.innerText = 'hide comments'
    } else {
        commentSection.classList.add('d-none');
        showCommentsButton.value = 'show comments'
        showCommentsButton.innerText = 'show comments'
    }
}

function countCharacters() {
    var text_length = $('#comment-content').val().length;
    var text_remaining = text_max - text_length;

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
    var counter = document.getElementById('counter');
    commentButtons.classList.add('d-none');
    counter.classList.add('d-none');
}

function showButtons() {
    var commentButtons = document.getElementById('comment-buttons');
    var counter = document.getElementById('counter');
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

function createThreadInPopup(categoryId) {
    $.ajax({
        type: 'GET',
        url: "/Threads/Create/",
        data: { categoryId: categoryId},
        success: function (res) {
            $('#staticBackdrop .modal-body').html(res);
            $('#staticBackdrop').modal('show');
        }
    })
}

createThreadPost = form => {
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

function deleteThreadInPopup(threadId, method ,categoryId,) {
    $.ajax({
        type: 'GET',
        url: "/Threads/Delete/",
        data: { threadId: threadId, method: method, categoryId: categoryId },
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal').modal('show');
        }
    })
}

function deleteThreadPost(threadId, method, categoryId,) {
    try {
        $.ajax({
            type: 'POST',
            url: '/Threads/Delete/',
            data: { threadId: threadId, method: method, categoryId: categoryId, },
            success: function (res) {
                if (res.method == 2) {
                    $('#read-container').html("");
                    $('#form-modal').modal('hide');
                    $('#staticBackdrop').modal('show');
                    $('#staticBackdrop .modal-body').html(res.html);
                }
                else {
                    document.getElementById(threadId).remove()
                    $.notify('The thread has been successfully deleted', { globalPosition: 'top center', className: 'success' });
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
                    $.notify('The comment has been successfully editted', { globalPosition: 'top center', className: 'success' });
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

function deleteCommentInPopup(commentId) {
    $.ajax({
        type: 'GET',
        url: "/Comments/Delete/",
        data: { commentId: commentId },
        success: function (res) {
            $('#form-modal .modal-body').html(res);
            $('#form-modal').modal('show');
        },
        error: function (err) {
            console.log(err);
        }
    })
}

function deleteCommentPost(commentId) {
    try {
        $.ajax({
            type: 'POST',
            url: '/Comments/Delete/',
            data: { commentId: commentId },
            success: function (res) {
                $('#comment-section').html(res.html);
                $('#form-modal').modal('hide');
                $.notify('The comment has been successfully deleted', { globalPosition: 'top center', className: 'success' });

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

function cancelModal() {
    $('#form-modal').modal('hide');
    $('#staticBackdrop').modal('hide');
}
