let device = {};
function loadSession(info) {

    device = JSON.parse(atob(info));

    var key = device._id;
    var stored = sessionStorage.getItem(key);

    if (!stored) {
        sessionStorage.setItem(key, stored = info);
    }
    device = JSON.parse(atob(stored));

    if (!device.setting) {
        device.setting = {};
    }
}
function saveSession() {
    var key = device._id;
    sessionStorage.setItem(key, btoa(JSON.stringify(device)));
}
function createPostData() {
    var data = {};
    var server = session.get("server");
    if (server) {
        data["#server"] = server;
    }
    data["#deviceId"] = device._id;
    return data;
}

function createPhoneSetting(name, caption) {
    var info = [];
    var key = name == 'sms' ? "SMS" : "CALL";
    var nums = device.setting[key];
    if (!nums) { nums = []; }

    for (var i = 0; i < 5; i++) {
        info.push({
            name: name + i,
            iconName: name,
            required: false,
            caption: '#' + (i + 1),
            value: nums[i],
        });
    }
    var frm = new ApiForm({
        header: caption,
        className: "info",
        url: "setting/" + key,
        sections: [
            {
                text: "Set the security phone numbers here",
                fields: info
            }],
    }).id(name).collapse(true);

    frm.setSubmit("return sendPhoneSetting('" + name + "')");

    for (const inp of frm.node.getElementsByClassName("input-group")) {
        inp.innerHTML += "<button class='btn' onclick='doCut(this); return false;'><i class='fa fa-cut'></i></button>"
    }

    render(frm);
}
function doCut(btn) {
    var p = btn.parentNode;
    var inp = p.getElementsByTagName("input")[0];
    inp.focus();
    inp.select();

    document.execCommand("cut");
}

function sendPhoneSetting(n) {
    var frm = document.getElementById(n);
    var inps = frm.getElementsByTagName("input");

    n = (n == 'sms' ? "SMS" : "CALL");

    var nums = [];
    for (const inp of inps) {
        nums.push(inp.value);
    }

    device.setting[n] = nums;
    api.post("setting/phone", {
        func: n,
        args: nums
    })
    //new API("setting/phone").post(postData);

    return false;
}

function updatePlan() {
    var p = currentDay.plan.join('');
    var index = currentDay.index;

    device.setting.plan[index] = p;

    saveSession();
    api.post("setting/plan", {
        func: "PLAN",
        args: [index, p]
    });
}

function updateCells() {
    for (const cell of cells) {
        var v = cell.id.split('_');
        cell.setAttribute("data-checked", currentDay.plan[v[0]] == v[1]);
    }
}

function setPlan(r, c) {
    for (var i = r; i < rows.length; i++) {
        currentDay.plan[i] = c;
    }
    updateCells();
}

function traversalDays(func) {
    var node = document.getElementById("day-menu").firstElementChild;
    var i = 0;
    while (node) {
        func(node, i++);
        node = node.nextElementSibling;
    }
}
function selectDay(d) {
    var i = d.getAttribute("data-index");
    if (i == currentDay.index) { return; }

    traversalDays((n, k) => n.classList.remove("active"));

    d.classList.add("active")

    if (!device.setting.plan) {
        device.setting.plan = {};
    }
    var p = device.setting.plan[i];
    if (!p) {
        p = device.setting.plan[7];
    }
    currentDay = {
        index: i,
        plan: p?.split('') ?? []
    };

    //if (i < 7) {
    //    currentDay.plan = device.setting.plan[i].split('');
    //}
    updateCells();
}

