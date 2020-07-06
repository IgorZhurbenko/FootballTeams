
function FillProperties(Donor, Acceptor, Except = []) {
    for (prop in Donor) {
        if (!Except.find(elem => elem == prop)) {
            Acceptor[prop] = Donor[prop].toString() == "[object Object]"? "" : Donor[prop];
        }
    }
}

function ClearVueData(VueObject)
{
    for (Field in VueObject._data)
    {
        VueObject[Field] = "";
    }
}

function Save(VueObject)
{
    let SentData = JSON.stringify(VueObject._data);
    let Request = new XMLHttpRequest();
    let Address = "/api/PlayersAPI"; 
    Request.open("POST", Address, true);
    Request.setRequestHeader('Content-Type', 'application/json; charset=UTF-8')
    Request.send(SentData);
    Request.onreadystatechange = function () {
        if (this.readyState == 4) {
            window.location.reload();
        }
    }

}

function CreateNew(VueObject)
{
    ClearVueData(VueObject);
    document.getElementById("PlayerInfo").hidden = false;
    document.getElementById("list").hidden = true;
}

function Return()
{
    //window.location.reload();
    document.getElementById("list").hidden = false;
    document.getElementById("PlayerInfo").hidden = true;
}

function Delete(id)
{
    let request = new XMLHttpRequest();
    request.open("DELETE", "api/PlayersAPI/" + String(id), true);
    request.send();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            //let responseObject = JSON.parse(this.responseText);
            ////console.log(JSON.stringify(Vue));
            //FillProperties(responseObject, VueObject, []);
            //document.getElementById("ContactInfo").hidden = false;
            //document.getElementById("list").hidden = true;
            ////console.log(JSON.stringify(Vue));
            window.location.reload();
        }
    }
}

function Edit(id, VueObject) {
    let request = new XMLHttpRequest();
    request.open("GET", "api/PlayersAPI/" + String(id), true);
    request.send();
    request.onreadystatechange = function () {
        if (this.readyState == 4) {
            let responseObject = JSON.parse(this.responseText);
            //console.log(JSON.stringify(Vue));
            FillProperties(responseObject, VueObject, []);
            document.getElementById("PlayerInfo").hidden = false;
            document.getElementById("list").hidden = true;
            VueObject.id = id;
            setTimeout(function () {
                SetValueFromList(document.getElementById('Edit_Team'), responseObject['TeamID'])
                SetValueFromList(document.getElementById('Edit_Gender'), responseObject['Gender'])
                }, 0.05);
            //console.log(JSON.stringify(Vue));
        }
    }
}

function SetValueFromList(ListElement, Value)
{
    for (let option of ListElement.options)
    {
        if (option.value == Value) { ListElement.selectedIndex = option.index; break; }
    }
}

//_________________________________________________________________________________________

function StartAddingTeam()
{
    let EditTeam = document.getElementById('Edit_Team');
    let NewTeamNameTextBox = document.getElementById('NewTeamNameTextBox');
    let StartEnteringButton = document.getElementById('StartEnteringTeamNameButton');
    let ConfirmButton = document.getElementById('ConfirmEnteredTeamNameButton');
    let CancelButton = document.getElementById('CancelEnteredTeamNameButton');

    EditTeam.hidden = true;
    NewTeamNameTextBox.hidden = false;
    StartEnteringButton.hidden = true;
    ConfirmButton.hidden = false;
    CancelButton.hidden = false;

    NewTeamNameTextBox.oninput = function ()
    {
        ConfirmButton.disabled = (this.value.length < 1);
    }
}

function CancelNewTeamCreation()
{
    let EditTeam = document.getElementById('Edit_Team');
    let NewTeamNameTextBox = document.getElementById('NewTeamNameTextBox');
    let StartEnteringButton = document.getElementById('StartEnteringTeamNameButton');
    let ConfirmButton = document.getElementById('ConfirmEnteredTeamNameButton');
    let CancelButton = document.getElementById('CancelEnteredTeamNameButton');

    EditTeam.hidden = false;
    NewTeamNameTextBox.hidden = true;
    StartEnteringButton.hidden = false;
    ConfirmButton.hidden = true;
    CancelButton.hidden = true;

    NewTeamNameTextBox.value = "";

}

function ConfirmNewTeam()
{
    let EditTeam = document.getElementById('Edit_Team');
    let NewTeamNameTextBox = document.getElementById('NewTeamNameTextBox');

    let SentData = JSON.stringify({ 'Name': NewTeamNameTextBox.value });
    let Request = new XMLHttpRequest();
    let Address = "/api/Teams";
    Request.open("POST", Address, true);
    Request.setRequestHeader('Content-Type', 'application/json; charset=UTF-8')
    Request.send(SentData);
    Request.onreadystatechange = function () {
        if (this.readyState == 4) {
            if (Number(this.responseText) == this.responseText)
            {
                let NewOption = document.createElement('option');
                NewOption.value = Number(this.responseText);
                NewOption.innerHTML = NewTeamNameTextBox.value;
                EditTeam.appendChild(NewOption);
            }
            CancelNewTeamCreation();
        }
    }
}

