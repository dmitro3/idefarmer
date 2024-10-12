mergeInto(LibraryManager.library, {
CreateInputFieldElement: function (inputFieldNamePtr) {
        var inputFieldName = UTF8ToString(inputFieldNamePtr);

        // Tạo một input element mới nếu không có
        if (!document.getElementById(inputFieldName)) {
            var inputElement = document.createElement("input");
            inputElement.type = "number";
            inputElement.id = inputFieldName;
            inputElement.style.position = "absolute";
            inputElement.style.left = "10px";
            inputElement.style.top = "10px";
            inputElement.style.width = "200px";
            document.body.appendChild(inputElement);
        }
    },
    ShowNumberInput: function (inputFieldNamePtr) {
        // Chuyển đổi chuỗi từ C# sang JavaScript
        var inputFieldName = UTF8ToString(inputFieldNamePtr);

        // Tìm đối tượng InputField của Unity trên HTML DOM
        var inputElement = document.getElementById(inputFieldName);
        if (inputElement) {
            // Thiết lập thuộc tính `type` cho input là `number` để nhập số thập phân
            inputElement.setAttribute("type", "number");
            inputElement.setAttribute("inputmode", "decimal"); // Chế độ nhập liệu thập phân
            inputElement.focus(); // Kích hoạt bàn phím ảo hoặc chế độ nhập liệu của trình duyệt
        }
    }
});
