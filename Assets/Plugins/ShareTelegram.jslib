mergeInto(LibraryManager.library, {
	
	 ShareOnTelegram: function(urlPtr,message) {

    // Chuyển đổi URL từ định dạng Unity sang JavaScript
    var url = UTF8ToString(urlPtr);

    // Kiểm tra và gọi hàm `openTelegramLink` nếu có sẵn
    if (window.Telegram && window.Telegram.WebApp && window.Telegram.WebApp.openTelegramLink) {
      window.Telegram.WebApp.openTelegramLink(url);
      console.log("Telegram link opened: " + url);
    } else {
      console.error("Telegram WebApp API is not available.");
    }
  }
  
});
