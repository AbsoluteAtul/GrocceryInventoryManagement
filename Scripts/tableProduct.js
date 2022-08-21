$(document).ready(function () {

    $(".click").click(function () {

        var proName = $("#ProductName").val();
        var proQuantity = $("#Quantity").val();
        var proBuying = $("#BuyingPrice").val();

        var code = "<tr><td><input type = 'checkbox' name='collect'/></td><td>" + proName + "</td><td>" + proQuantity + "</td><td>" + proBuying + "</td></tr>";

        $("table .tbody").append(code);

    })

    $(".del").click(function () {

        $("table .tbody").find('input[name="collect"]').each(function (){

            if ($(this).is(":checked")) {
                $(this).parents("tr").remove();
            }

        })

    });
});