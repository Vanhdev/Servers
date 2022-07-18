function setCookie(cname, cvalue, exdays) {
    const d = new Date();
    d.setTime(d.getTime() + ((exdays ?? 1) * 24 * 60 * 60 * 1000));
    let expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var decodedCookie = decodeURIComponent(document.cookie);
    var ca = decodedCookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function API(action) {

    var http = new XMLHttpRequest();
    http.onload = function () {
        var e = JSON.parse(http.response);
        processApiResponse(e);
    }

    var url = "/api/common";

    this.post = function (data) {

        if (!data) { data = {}; }

        data["#url"] = action;
        let token = getCookie("token");
        if (token) {
            data["#token"] = token;
        }

        http.open('POST', url)
        http.setRequestHeader('Content-type', 'application/json')
        http.send(JSON.stringify(data)) // Make sure to stringify
    }
}

function processApiResponse(context) {

    if (context.code) {
        raiseApiResponseError(context.code, context.message);
    }
    else {
        raiseApiResponseOK(context.value);
    }
}
function raiseApiResponseError(code, message) {
    toastr.error("Code " + code + ": " + message);
}
function raiseApiResponseOK(value) {
    toastr.success("Cập nhật thành công");
}
function redirect(url) {
    var a = document.body
        .appendChild(document.createElement("a"));
    a.href = url;
    a.click();
}

function inputTag(name) {
    tag.apply(this, [name]);
    this.value = function (v) {
        this.attr("value", v);
        return this;
    }
    this.valueOf = function () {
        var v = this.node.value;
        if (!v && this.node.required) { return null; }
        return v == null ? "" : v;
    }
    this.required = function (b) {
        this.node.required = b;
        return this;
    }
}
function input() {
    inputTag.apply(this, ["input"]);
}
function checkbox() {
    tag.apply(this, ["div"]);

    this.html("<span><i class='fa fa-check'></i></span>").addClass("check-box");
    var lab = this.child(new tag("label").html("Caption"));
    var inp = this.child(new input().type("checkbox"));

    this.node.addEventListener("click", function () {
        inp.node.click();
    });
    inp.event("change", function (n) {
        n.parentElement.setAttribute("data-checked", n.checked);
        inp.value(n.checked);
    });

    this.caption = function (t) {
        lab.html(t);
    }
    this.name = function (n) {
        inp.name(n);
    }
    this.value = function (v) {
        var b = v != null && (v == 1 || v == true || v == 'on' || v.indexOf('u') > 0);

        inp.node.checked = b;
        inp.value(inp.node.checked);

        this.data("checked", inp.node.checked);
    }
    this.valueOf = function () { return inp.node.checked; }
}
function select() {
    inputTag.apply(this, ["select"]);
    this.value = function (v) {
        console.log(v);
        for (const opt of this.node.childNodes) {
            if (v == opt.innerText) {
                opt.setAttribute("selected", true);
                return;
            }
        }
    }
}

function date() {
    input.apply(this);
    this.addClass("date");
}
function createControl(field) {

    if (!field.caption) {
        field.caption = field.name;
    }

    var row = new tag()
        .addClass(field.className ?? "col-12")
        .addClass("input-group mb-3");
    var inp = new input();
    var type = field.input;
    if (type) {
        if (window[type]) {
            inp = new window[type]();
        }
        else {
            inp.attr("type", type)
        }
    }

    inp.attr("placeholder", field.caption)
        .required(true)
        .id(field.name)
        .name(field.name);

    if (field.required == false) {
        inp.required(false);
    }
    if (field.options) {
        for (const s of field.options.split(';')) {
            inp.child(new tag("option").html(s));
        }
    }

    //var label = new tag("label")
    //    .html(field.caption)
    //    .attr("for", field.name);


    if (field.iconName) {
        row.child(new panel("input-group-prepend"))
            .child(new panel("input-group-text"))
            .html("<i class='fa fa-" + field.iconName + "'></i>");
    }
    inp.value(field.value);
    row.child(inp.addClass("form-control"));

    return row;
}

function panel(cls) {
    tag.apply(this);
    this.addClass(cls ?? "row");

    this.content = function (obj) {
        if (typeof obj === "string") {
            this.html(obj);
        } else if (typeof obj === "Node") {
            this.node.appendChild(obj);
        } else {
            this.child(obj);
        }
        return this;
    }
}
function render(t) {
    document.write(t.node.outerHTML);
}

function setDateInputMask(inp) {
    var today = new Date();

    if (inp.value) {
        var v = inp.value.split('-');
        var t = v[0];
        v[0] = v[2];
        v[2] = t;
        inp.value = v.join('/');

    }

    inp.caption = inp.getAttribute("placeholder");
    inp.addEventListener("blur", function () {
        inp.setAttribute("placeholder", inp.caption);
        if (inp.value) {
            var v = inp.value
                .replaceAll(' ', '/')
                .replaceAll('.', '/')
                .replaceAll('-', '/')
                .split('/');

            if (v.length < 2) {
                var m = today.getMonth() + 1;
                v[1] = m < 10 ? '0' + m : m;
            }
            if (v.length < 3) {
                v[2] = today.getFullYear();
            }
            if (v[0].length < 2) { v[0] = '0' + v[0]; }
            if (v[1].length < 2) { v[1] = '0' + v[1]; }

            var y = parseInt(v[2]);
            if (y < 30) {
                v[2] = y + 2000;
            } else if (y < 100) {
                v[2] = y + 1900;
            }
            inp.value = v.join('/');
        }
    });
    inp.addEventListener("focus", function () {
        inp.setAttribute("placeholder", "dd/mm/yyyy");
    });

    inp.valueOf = function () {
        if (!inp.value) return null;

        var v = inp.value.split('/');
        var t = v[0];
        v[0] = v[2];
        v[2] = t;
        return v.join('-');
    }
}

function callSubmit(formId) {
    var data = {
        value: {}
    };
    var frm = document.getElementById(formId);

    var inps = frm.getElementsByClassName("form-control");
    for (const inp of inps) {
        var v = inp.valueOf();
        if (!v && inp.hasAttribute("required")) {
            return false;
        }

        var id = inp.id;
        if (id[0] == '#') {
            data[id] = v;
        }
        else {
            data.value[id] = v;
        }
    }
    var api = new API(frm.getAttribute("action"));
    api.post(data);
    return true;
}

function ApiForm(info) {

    Card.apply(this, [info.className ?? "indigo"]);
    this.addClass("form");

    var id = info.url.replace('/', '_');
    var frm = new tag("form")
        .attr("method", "post")
        .attr("action", info.url)
        .id(id);

    for (const section of info.sections) {
        var sectionText = frm.child(new tag().addClass("section-text"));
        sectionText.html(section.text ?? "");

        var row = frm.child(new panel());
        for (const field of section.fields) {
            row.child(createControl(field));
        }
    }

    var submit = new tag("button")
        .attr("onclick", "return !callSubmit('" + id + "')")
        .id(id + '_submit');
    frm.child(submit).node.hidden = true;

    this.addClass("mb-3")
        .header(info.header ?? 'header: undefined');

    this.body(frm);

    var submitAlias = new tag("button")
        .addClass("btn btn-" + (info.className ?? "primary"))
        .attr("type", "submit")
        .html("Submit")
        .attr("onclick", id + "_submit.click()");
    this.setSubmit = function (onclick) {
        submit.attr("onclick", onclick);
    }
    this.footer(submitAlias);
}


function initForms() {
    var forms = document.getElementsByTagName("form");
    var index = 0;

    for (const frm of forms) {
        var inps = frm.getElementsByClassName('form-control');
        for (const inp of inps) {
            if (inp.classList.contains("date")) {
                setDateInputMask(inp);
                continue;
            }

            inp.valueOf = function () { return inp.value; }
        }

        //    var btns = frm.getElementsByTagName("button");
        //    for (const btn of btns) {
        //        if (btn.getAttribute("type") == "submit") {
        //            frm.id = "form-" + (++index);
        //            btn.setAttribute("onclick", "return !callSubmit('" + frm.id + "')");
        //        }
        //    }
    }
}

function Card(cls) {
    tag.apply(this);
    this.addClass("card card-" + (cls ?? "default"));

    var head = this.child(new panel("card-header"));
    this.header = function (obj) {
        head.content(obj);
        return this;
    }
    this.body = function (obj) {
        this.child(new panel("card-body").content(obj));
        return this;
    }
    this.footer = function (obj) {
        this.child(new panel("card-footer").content(obj));
        return this;
    }

    var menu = null;
    this.tool = function (obj) {
        if (!menu) {
            menu = head.child(new panel("card-tools"));
        }
        menu.content(obj);
        return this;
    }
    this.collapse = function (collapsed) {
        if (collapsed) {
            this.addClass("collapsed-card");
        }

        var btn = new tag("button")
            .addClass("btn btn-tool")
            .data("card-widget", "collapse")
            .attr("title", "Collapse");
        btn.child(new tag("i").addClass("fas")
            .addClass("fa-" + (collapsed ? 'plus' : 'minus')));

        return this.tool(btn);
    }
}

function beginSearch() {
    //var href = window.location.pathname;
    var v = window.location.pathname;
    var i = v.indexOf('/', 1);
    if (i < 0) {
        v = "/home";
    }
    else {
        v = v.substr(0, i);
    }
    document.getElementById("search-form").action = v + "/search";
}

function ApiTable(info) {

    Card.apply(this);
    var table = new tag("table").addClass("table");
    var thead = table.child(new tag("thead"));
    var tbody = table.child(new tag("tbody"));

    var tr = thead.child(new tag("tr"));
    for (const col of info.fields) {
        var th = tr.child(new tag("th").html(col.caption));
    }

    var url = info.url;
    if (!url) {
        url = window.location.href;
        var i = url.lastIndexOf('/');
        var k = url.indexOf('?');
        if (k < 0) {
            k = url.length;
        }
        url = url.substring(i, k);
        console.log(url);
    }
    var header = new panel();
    header.child(new tag().addClass("table-caption col-6").html(info.header));
    header.child(new tag().addClass("table-action col-6").html("<a class='btn-sm btn-primary' href='" + url + "/edit'><i class='fa fa-plus' /></a>"));

    this.header(header);
    this.body(table).addClass("p-0");
}