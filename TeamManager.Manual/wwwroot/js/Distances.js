function addRaceDistanceButtonClick() {

    var newDistanceDiv = document.createElement("div");
    newDistanceDiv.setAttribute("class", "col col-sm-1");

    var newDistanceField = document.createElement("input");
    newDistanceField.setAttribute("type", "number");
    newDistanceField.setAttribute("step", ".1");
    newDistanceField.setAttribute("name", "DistanceLengths");
    newDistanceField.setAttribute("id", "DistanceLengths");
    newDistanceField.setAttribute("pattern", "^\d*(\.\d{0,1})?$");
    newDistanceField.setAttribute("class", "m-2 form-control");

    newDistanceDiv.appendChild(newDistanceField);

    document.getElementById("distances-container").appendChild(newDistanceDiv);
}

function removeRaceDistanceButtonClick() {

    var container = document.getElementById("distances-container");
    var items = container.childNodes.length;

    // Container has 3 childs by default
    if (items > 3) {
        container.removeChild(container.childNodes[items - 1]);
    }
}