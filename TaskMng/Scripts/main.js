function getStatuses(handler) {
    $.ajax({
        url: "/Home/GetStatuses",
        method: "GET"
    })
    .done(function (data) {
        handler(data);
    });
}


function getPossibleMembers() {
    $.ajax({
        url: "/Home/GetPossibleMembers",
        method: "GET"
    })
        .done(function (data) {
            $("#list-programmers").html(data);
        })
        .fail(function () {
            alert("Cannot do it");
        })
}

function setAddedMembers() {
    $("#add-member").prop("disabled", ($("input:checkbox:checked").length === 0));
}

function addMember() {
    var members = [];
    $("input:checkbox:checked").each(function () {
        var checkEl = $(this);
        if (checkEl.prop("checked")) {
            members.push(checkEl.val());
        }
    })
    $.ajax({
        url: "/Home/AddMembersToTeam",
        method: "POST",
        data: { persons: members }
    })
        .done(function (Msg) {
            alert(Msg);
            $("#myTeamBtn").click();
        })
}

function deleteFromTeam(personId) {
    $.ajax({
        url: "/Home/DeleteFromTeam",
        method: "POST",
        data: { id: personId }
    })
        .done(function (Msg) {
            alert(Msg);
            $("#person" + personId).remove();

        })
        .fail(function () {
            alert("Cannot delete this member!");
        });
}

function showStatuses(id) {
    $.ajax({
        url: "/Home/GetStatuses",
        method: "GET"
    })
        .done(function (data) {
            var status = $("#status" + id);
            status.html(data);
            status.val(status.data("status"));
        });
}

function saveParentTaskId(parentId) {
    $("#add-task").data("parentTaskId", parentId);
    $("#assignee-create-task").prop("hidden", parentId);
    $("#span-assignee").prop("hidden", parentId);
    $("#span-assignee").data("add-subtask", true);
}

function editTask(taskId) {
    $.ajax({
        url: "/Home/EditTask",
        method: "POST",
        data: { id: taskId }
    })
        .done(function (data) {
            $("#edit-form").html(data);
            $("#save-task").data("editTaskId", taskId);
        })
        .fail(function () {
            $("#edit-form").html("<p>Cannot find task</p>");
        });
}

function details(mainId) {
    $.ajax({
        url: "/Home/Details",
        method: "GET",
        data: { id: mainId },
    })
        .done(function (data) {
            $("#detailOfTask").html(data);
        })
        .fail(function () {
            alert("Cannot find the task");
        });
}

function deleteTask(taskId) {
    $.ajax({
        url: "/Home/DeleteTask",
        method: "POST",
        data: { id: taskId }
    })
        .done(function () {
            alert("Task has been deleted.");
            $("#task" + taskId).remove();
        })
        .fail(function () {
            alert("Cannot delete the task!");
        });
}

function showSubtasks(taskId) {
    $.ajax({
        url: "/Home/ShowSubtask",
        method: "GET",
        data: { parentId: taskId }
    })
        .done(function (data) {
            var taskRow = $("#task" + taskId);
            taskRow.after(data);

            $("#iconshow-subtask", taskRow).hide();
            $("#iconhide-subtask", taskRow).show();

            getStatuses(function (statuses) {
                setStatuses(statuses, taskRow.next().find("select"));
            });
        })
}

function hideSubtasks(taskId) {
    var taskRow = $('#task' + taskId);
    taskRow.next().remove();
    $("#iconhide-subtask", taskRow).hide();
    $("#iconshow-subtask", taskRow).show();
}

function setStatuses(statuses, selects) {
    selects.html(statuses);
    $.each(selects, function () {
        var $this = $(this);
        $this.val($this.data("status"));
    });
}

function showAssignees(managerId, selector) {
    if (!managerId) {
        return;
    }

    $.ajax({
        url: "/Home/GetAssignees",
        method: "GET",
        data: { managerId: managerId }
    })
        .done(function (data) {
            var assignee = $(selector);
            assignee.html(data);
            assignee.val(assignee.data("assignee"));
        });
}

function updateAssignee(selector) {
    var assignee = $(selector);
    assignee.data("assignee", assignee.val());
}
