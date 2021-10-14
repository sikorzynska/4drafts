// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

//$(document).ready(function () {
//    $(".pointthis").on("load", function () {
//        console.log('hello');
//        appendSelect('g', 'b');
//    });
//});

//function paging() {
//    $('#threadTable').DataTable({
//        "scrollCollapse": true,
//        "paging": true
//    });
//}

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();
    $('#content-content').summernote({
        height: 135,
        toolbar: [
            // [groupName, [list of button]]
            ['style', ['style', 'bold', 'italic', 'underline', 'clear']],
            ['font', ['strikethrough', 'superscript', 'subscript']],
            ['insert', ['link']],
            ['view', ['codeview', 'help']]
        ]
    });
});

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


$("[data-toggle=popover]").popover();

function countCharacters(counterId, valueId, max) {
    var text_length = $(document.getElementById(valueId)).val().length;

    var counter = document.getElementById(counterId);
    $(counter).html(text_length + ' / ' + max);
};

function changeArrow(arrow, box = null, prop = null) {
    var foldArrow = document.getElementById(arrow);
    if (box != null && prop != null) {
        var contentBody = document.getElementById(box);
        if (contentBody.classList.contains(prop)) {
            foldArrow.classList.remove('fa-arrow-down');
            foldArrow.classList.add('fa-arrow-up');
            contentBody.classList.remove(prop);
        } else {
            foldArrow.classList.remove('fa-arrow-up');
            foldArrow.classList.add('fa-arrow-down');
            contentBody.classList.add(prop);
        }
    }
    else {
        if (foldArrow.classList.contains('fa-arrow-down')) {
            foldArrow.classList.remove('fa-arrow-down');
            foldArrow.classList.add('fa-arrow-up');
        }
        else {
            foldArrow.classList.remove('fa-arrow-up');
            foldArrow.classList.add('fa-arrow-down');
        }
    }
}

function isEmpty(str) {
    return !$.trim(str).length;
}

function checkIfEmpty() {
    var commentButton = document.getElementById('comment-button');
    var commentContent = document.getElementById('comment-content');

    if (!commentContent.value.length == 0 && /\S/.test(commentContent.value)) {
        commentButton.classList.remove('d-none');
    } else {
        commentButton.classList.add('d-none');
    }
};

function showCommentRow() {
    var commentRow = document.getElementById('comment-row');
    commentRow.classList.remove('d-none');
}

function hideCommentRow() {
    var commentRow = document.getElementById('comment-row');
    commentRow.classList.add('d-none');
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
                hideCommentRow();
            }
        }
    });
}

function refer(id) {
    var form = document.getElementById(id);

    switch (id) {
        case 'thread-create-formdata': {
            createPost(form);
            break;
        }
        case 'profile-manage-formdata': {
            managePost(form);
            break;
        }
        case 'login-formdata': {
            authPost(form);
            break;
        }
        case 'register-formdata': {
            authPost(form);
            break;
        }
        case 'thread-edit-formdata': {
            editPost(form);
            break;
        }
        case 'comment-edit-formdata': {
            editPost(form);
            break;
        }
        case 'draft-edit-formdata': {
            editPost(form);
            break;
        }
        default: {
            console.log('default')
            break;
        }
    }
}

function popUp(path, type = null, returnUrl = null, entity = null, tt = null) {
    switch (type) {
        case 'drafts': {
            $.ajax({
                type: 'GET',
                url: path,
                data: { typeId: tt },
                success: function (res) {
                    $('#form-modal .modal-body').html(res);
                    $('#form-modal').modal('show');
                },
                error: function (err) {
                    console.log(err);
                }
            })
            break;
        }
        case 'auth': {
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
            break;
        }
        case 'thread-new': {
            $.ajax({
                type: 'GET',
                url: path,
                data: { tt : tt },
                success: function (res) {
                    $('#staticBackdrop .modal-body').html(res);
                    $('#staticBackdrop').modal('show');
                    $('.selectpicker').selectpicker();
                },
                error: function (err) {
                    console.log(err);
                }
            })
            break;
        }
        case 'thread-draft': {
            $.ajax({
                type: 'GET',
                url: path,
                data: { draftId: entity, tt: tt },
                success: function (res) {
                    $('#staticBackdrop .modal-body').html(res);
                    $('#staticBackdrop').modal('show');
                    $('.selectpicker').selectpicker();
                },
                error: function (err) {
                    console.log(err);
                }
            })
            break;
        }
        case 'draft-edit': {
            $.ajax({
                type: 'GET',
                url: path,
                data: { Id: entity },
                success: function (res) {
                    if (res.isValid) {
                        $('#staticBackdrop .modal-body').html(res.html);
                        $('#staticBackdrop').modal('show');
                        $('.selectpicker').selectpicker();
                    }
                    else {
                        $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
                        $('#staticBackdrop').modal('hide');
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            })
            break;
        }
        case 'peek-profile': {
            $.ajax({
                type: 'GET',
                url: path,
                data: { u: entity },
                success: function (res) {
                    if (res.isValid) {
                        $('#form-modal .modal-body').html(res.html);
                        $('#form-modal').modal('show');
                    }
                    else {
                        $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            })
            break;
        }
        case 'thread-edit': {
            $.ajax({
                type: 'GET',
                url: path,
                data: { Id: entity },
                success: function (res) {
                    if (res.isValid) {
                        $('#staticBackdrop .modal-body').html(res.html);
                        $('#staticBackdrop').modal('show');
                    }
                    else {
                        $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
                        $('#staticBackdrop').modal('hide');
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            })
            break;
        }
        case 'comment-edit': {
            $.ajax({
                type: 'GET',
                url: path,
                data: { Id: entity },
                success: function (res) {
                    if (res.isValid) {
                        $('#staticBackdrop .modal-body').html(res.html);
                        $('#staticBackdrop').modal('show');
                    }
                    else {
                        $.notify(res.msg, { globalPosition: 'top left', className: 'error' });
                        $('#staticBackdrop').modal('hide');
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            })
            break;
        }
        case 'account-manage': {
            $.ajax({
                type: 'GET',
                url: path,
                success: function (res) {
                    $('#staticBackdrop .modal-body').html(res);
                    $('#staticBackdrop').modal('show');
                },
                error: function (err) {
                    console.log(err);
                }
            })
            break;
        }
    }
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

function managePost(form) {
    $.ajax({
        type: 'POST',
        url: form.action,
        data: new FormData(form),
        contentType: false,
        processData: false,
        success: function (res) {
            if (res.isValid) {
                $('.notifyjs-corner').empty();
                $('#staticBackdrop').modal('hide');
                $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
            }
            else {
                $('#staticBackdrop .modal-body').html(res.html);
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
                    $('.notifyjs-corner').empty();
                    $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
                    $('#draft-count').html(res.count);
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
                    $('.selectpicker').selectpicker();
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

function likeThread(threadId, fromList) {
    $.ajax({
        type: 'Post',
        url: "/Threads/Like/",
        data: { threadId: threadId, fromList: fromList },
        success: function (res) {
            if (res.fromList) {
                var heart = document.getElementById(res.heart);
                if (res.liked) {
                    heart.classList.remove('far');
                    heart.classList.add('fas');
                    heart.classList.add('text-danger');
                }
                else {
                    heart.classList.remove('fas');
                    heart.classList.remove('text-danger');
                    heart.classList.add('far');
                }
                $('.notifyjs-corner').empty();
                $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
            }
            else {
                $('#thread-likes-section').html(res.html);
                $('.notifyjs-corner').empty();
                $.notify(res.msg, { globalPosition: 'top left', className: 'success' });
            }
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

function getValue(id) {
    var content = document.getElementById(id);
    var result = $(content).val();
    return result;
}

function getSelectedValue(id) {
    var elem = document.getElementById(id);
    var selected_option = $(elem).find(":selected");
    var selected_value = selected_option.val();
    return selected_value;
}

function IsEmpty(str) {
    if (/\S/.test(str)) {
        return false;
    }
    return true;
}


function addFilterRoutes() {
    var filterBtn = document.getElementById('filter');
    var type = IsEmpty(getSelectedValue('type-select')) == false ? '&type=' + getSelectedValue('type-select') : null;
    var genre = IsEmpty(getSelectedValue('genre-select')) == false ? '&genre=' + getSelectedValue('genre-select') : null;
    var sort = IsEmpty(getSelectedValue('sort-select')) == false ? '&sort=' + getSelectedValue('sort-select') : null;
    var liked = getValue('is_liked') == 'True' ? '&liked=true' : null;
    var user = IsEmpty(getValue('is_u')) == false ? '&u=' + getValue('is_u') : null;

    var path = '/threads/browse?page=1';

    if (type != null) path += type;
    if (genre != null) path += genre;
    if (sort != null) path += sort;
    if (liked != null) path += liked;
    if (user != null) path += user;

    $(filterBtn).attr('href', path);
}

//$(".divider-line").ready(function () {
//    var genre = $('#genre-value').val() + 'genre';
//    var sort = $('#sort-value').val();
//    var type = $('#type-value').val() + 'type';

//    var genreOption = document.getElementById(genre);
//    var sortOption = document.getElementById(sort);
//    var typeOption = document.getElementById(type);

//    console.log(genre);
//    console.log(sort);
//    console.log(type);

//    $(genreOption).attr('selected', "");
//    $(sortOption).attr('selected', "");
//    $(typeOption).attr('selected', "");
//});

function saveDraft(title, content, genreIds, typeId, draftId, promptId) {
    try {
        $.ajax({
            type: 'POST',
            url: '/Drafts/Save/',
            data: { title: title, content: content, genreIds: genreIds, typeId: typeId, draftId: draftId, promptId: promptId },
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
