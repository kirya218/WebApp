function isNumberKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode
    if (charCode > 31 && (charCode < 49 || charCode > 57))
        return false;
    return true;
}

function showCandidate() {
    let tableAnswer = document.getElementById("table-answer");
    tableAnswer.style.display = "block";

    $("input").attr("disabled", true)
}

function checkSolution() {
    let tableList = new Array();
    let table = document.getElementsByClassName("table");

    for (let i = 0; i < table.length; i++) {

        let tr = table.item(i).getElementsByTagName("tr");
        console.log(table.item(i));
        tableList[i] = new Array();

        for (let j = 0; j < tr.length; j++) {

            let td = tr.item(j).getElementsByTagName("td");
            tableList[i][j] = new Array();

            for (let f = 0; f < td.length; f++) {
                let input = td.item(f).getElementsByTagName("input");
                tableList[i][j][f] = input.item(0).value;
            }
        }
    }

    const solution = tableList[0];

    
}