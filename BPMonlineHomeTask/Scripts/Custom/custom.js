$(document).ready(function() {
    $(".contact-update").click(function () {

        console.log("CLICK");
        if ($(this).hasClass("active")) {
            $(this).parent().find(".contact-item-update").hide(400);
            $(this).removeClass("active");
        } else {
            $(this).parent().find(".contact-item-update").show(400);
            $(this).addClass("active");
        }
        
        
    });

    $(".contact-delete").click(function () {

        console.log("CLICK");
        if ($(this).hasClass("active")) {
            $(this).parent().find(".contact-item-delete").hide(400);
            $(this).removeClass("active");
        } else {
            $(this).parent().find(".contact-item-delete").show(400);
            $(this).addClass("active");
        }
    });
    $(".delete-item-no").click(function () {
        $(this).parent().parent().find(".contact-delete").click();
    });

    //DELETE CONTACT
    $(".delete-contact-form").submit(function (e) {

        e.preventDefault();
        var flag = false;
        var url = $(this).attr("action");
        var id = $(this).find("input[name=Id]").val();

        console.log(url);
        console.log(id);

        $.ajax({
            type: "POST",
            url: url,
            data: { 'id': id },
            success: function (msg) {
                console.log("WORKED!");
                $(".delete-contact-form-message").text(msg);
            }
        }).error(function(msg) {
            console.log("Some problems on server.");
            $(".delete-contact-form-message").text(msg);
            return;
        });

        $("div[id=" + id + "]").hide(1000);
    });
    
    /*
    $.ajax({
        type: "POST",
        url: "MessagePopup.aspx/SendMessage",
        data: "{subject:'" + subject + "',message:'" + message + ",messageId:'" + messageId + "',pupilId:'" + pupilId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        
    });*/

    //CREATE CONTACT
    $(".create-new-contact-form").submit(function (e) {
        e.preventDefault(); // avoid to execute the actual submit of the form.

        var url = $(this).attr("action");

        var name = $(this).find("input[name=Name]").val();
        var phone = $(this).find("input[name=MobilePhone]").val();
        var dear = $(this).find("input[name=Dear]").val();
        var jobTitle = $(this).find("input[name=JobTitle]").val();
        var birthDate = $(this).find("input[name=BirthDate]").val();

        console.log(name);
        console.log(phone);
        console.log(dear);
        console.log(jobTitle);
        console.log(birthDate);
        
        var flag = true;
        var inputs = $(this).find("input");
        console.log(inputs.length);
        inputs.each(function() {
            if (validationForm($(this)) == false)
                flag = false;
            return;
        });
        if (flag) {

            $.ajax({
                type: "POST",
                url: url,
                data: {
                    Name: name,
                    MobilePhone: phone,
                    Dear: dear,
                    JobTitle: jobTitle,
                    BirthDate: birthDate
                }

            }).done(function (data) {
                $(".new-contact-form-message > p > span").text(data);
                clearForm($("form.create-new-contact > input"));
            }).error(function (data) {
                $(".new-contact-form-message > p > span").text(data);
            });
        } else {
            $(".new-contact-form-message > p > span").text("Check the form!");
            return;
        }

        clearForm(inputs);

    });

    function clearForm(inputs) {
        inputs.each(function () {
            if ($(this).attr("name") != "submit") {

                $(this).val("");
            }
        });
    }

    //UPDATE CONTACT
    $(".contact-item-update-form").submit(function (e) {
        e.preventDefault();

        var url = $(this).attr("action");

        var id = $(this).find("input[name=Id]").val();
        var name = $(this).find("input[name=Name]").val();
        var phone = $(this).find("input[name=MobilePhone]").val();
        var dear = $(this).find("input[name=Dear]").val();
        var jobTitle = $(this).find("input[name=JobTitle]").val();
        var birthDate = $(this).find("input[name=BirthDate]").val();

        var flag = true;
        var inputs = $(this).find("input");
        console.log(inputs.length);
        inputs.each(function() {
            if (validationForm($(this)) == false)
                flag = false;
                return;
        });
        if (flag) {
            $.ajax({
                type: "POST",
                url: url,
                data: {
                    Id: id,
                    Name: name,
                    MobilePhone: phone,
                    Dear: dear,
                    JobTitle: jobTitle,
                    BirthDate: birthDate
                }
            }).done(function (resp) {
                console.log("NICE!");
                $(".contact-item-update-form-message").text(resp);
            }).error(function(resp) {
                console.log("ERROR!");
                $(".contact-item-update-form-message").text(resp);
                return;
            });
        } else {
            $(".contact-item-update-form-message").text("Check the form!");
            return;
        }

        var viewItems = $(this).parent().parent().find(".contact-item-view");

        viewItems.find("span[name=Name]").text(name);
        viewItems.find("span[name=Phone]").text(phone);
        viewItems.find("span[name=Dear]").text(dear);
        viewItems.find("span[name=JobTitle]").text(jobTitle);
        viewItems.find("span[name=BirthDate]").text(birthDate);

    });
});

function validationForm(obj) {

    var textRegEx = /^[a-zA-Z\s]*$/; 
    var phoneRegEx = /^[0-9-+]+$/;
    var flag = false;

    switch (obj.attr("name")) {
        case "Name":
            if (textRegEx.test(obj.val())) {
                flag = true;
            }
            break;
        case "MobilePhone":
            if (phoneRegEx.test(obj.val())) {
                flag = true;
            }
            break;
        case "Dear":
            if (textRegEx.test(obj.val())) {
                flag = true;
            }
            break;
        case "JobTitle":
            if (textRegEx.test(obj.val())) {
                flag = true;
            }
            break;
        case "BirthDate":
                flag = true;
                break;
        case "submit":
            flag = true;
            break;
        case "Id":
            flag = true;
            break;
    }

    if (!flag) {
        obj.css("border-color", "red");
    }else {
        obj.css("border-color", "");
    }

    return flag;
}