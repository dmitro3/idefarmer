mergeInto(LibraryManager.library, {

InitializeTonConnect: function () {
    if (window.tonConnectUI === undefined) {
      // Tạo đối tượng TonConnectUI với manifest URL đã chỉ định
        window.tonConnectUI = new TON_CONNECT_UI.TonConnectUI({
            manifestUrl: 'https://game.tonthesheep.com/tonconnect.json'
        });
	  	window.tonConnectUI.uiOptions = {
			  twaReturnUrl: 'https://t.me/tonthesheep_bot/game'
		  };
      console.log("TonConnect UI initialized.");

	  // Sử dụng `connectionRestored` để kiểm tra trạng thái khôi phục
        window.tonConnectUI.connectionRestored
            .then((restored) => {
                if (restored && window.tonConnectUI.wallet) {
                    var walletAddress = window.tonConnectUI.wallet.account.address;
                    console.log("Restored Wallet Address:", walletAddress);
                    // Gửi địa chỉ ví đã khôi phục về Unity
                      unityInstanceRef.SendMessage("TonWalletconnect", "OnTonConnectInitialized", walletAddress);
                } else {
                    console.log("No Wallet Connection Restored");
                    unityInstanceRef.SendMessage("TonWalletconnect", "OnTonConnectInitialized", "0");
                }
            })
            .catch((err) => {
                console.error("Failed to restore wallet connection", err);
                 unityInstanceRef.SendMessage("TonWalletconnect", "OnTonConnectInitialized", "0");
            });
			
	  

    } else {
      console.log("TonConnect UI is already initialized.");
       unityInstanceRef.SendMessage("TonWalletconnect", "OnTonConnectInitialized", "0");
    }
  },
  
  ConnectTonWallet: function () {
		
	window.tonConnectUI.connectWallet().then((walletInfo) => {
           
		   	if (walletInfo && walletInfo.account) 
			{
				var walletAddress = walletInfo.account.address; // Lấy địa chỉ ví
				console.log(walletAddress);
				unityInstanceRef.SendMessage("TonWalletconnect", "OnTonConnected", walletAddress);
				
			}else{
			
				unityInstanceRef.SendMessage("TonWalletconnect", "OnTonConnected", "0");
			}

			
        }).catch((error) => {
		
            console.log("Lỗi khi kết nối ví:", error);
			unityInstanceRef.SendMessage("TonWalletconnect", "OnTonConnected", "0");
        
		});
  },
  
   DisConnectTonWallet: function () {
		

		if (window.tonConnectUI) {
		
			window.tonConnectUI.disconnect().then(() => {
			console.log("Wallet Disconnected Successfully");
			unityInstanceRef.SendMessage("TonWalletconnect", "OnTonDisConnected", "1");
			 
			}).catch((error) => {
			console.log("Error disconnecting wallet: ", error);
			 unityInstanceRef.SendMessage("TonWalletconnect", "OnTonDisConnected", "0");
			});
		} else {
		
			console.error("TonConnectUI is not initialized!");
			unityInstanceRef.SendMessage("TonWalletconnect", "OnTonDisConnected", "0");
		}


  
  },
  
	  
	  BuyFarmTool: async  function ( addressdeposit, amout, data){
	  
	  
	    var addressdepositAd = UTF8ToString(addressdeposit);
        var amoutAd = UTF8ToString(amout);
		  var dataAd = UTF8ToString(data);
			
			const body = new TonWeb.boc.Cell(); // Tạo một Cell mới để chứa dữ liệu
			body.bits.writeUint(0, 32);
			body.bits.writeString(dataAd);
			
			// Chuyển đổi dữ liệu thành Base64 để tạo payload
			const payload = TonWeb.utils.bytesToBase64(await body.toBoc());
			
			// Cấu hình giao dịch
			const transaction = {
			  validUntil: Math.floor(Date.now() / 1000) + 180, // 60 giây
			  messages: [
				{
				  address: addressdepositAd, // Địa chỉ ví nhận
				  amount: amoutAd, // Số tiền gửi (tính bằng nanoTON)
				  payload: payload,
				}
			  ]
			};

			// Gửi giao dịch thông qua tonConnectUI
			try {
			  const result = await window.tonConnectUI.sendTransaction(transaction);
			  //alert('Transaction was sent successfully');
			} catch (error) {
			  //alert("Transaction failed: " + error);
			}
				
	  },

	socketConnect: function(serverUrl, token) {
	
	
		 serverUrl = UTF8ToString(serverUrl);
		 token = UTF8ToString(token);
	 
	  // Convert the C# pointers to JavaScript strings


        // Create the socket connection with the provided server URL and authentication token
        var socket = io(serverUrl+"", {
            auth: {
                authorization: token+""
            },
            upgrade: false
        });
		
		
		
        
        // Attach events for Socket.IO
        socket.on('connect', function () {
            console.log("Connected to server");
            // Trigger a Unity callback if needed
            //SendMessage("GameObjectName", "OnSocketConnect", "Connected");
        });

        socket.on('rentShepherd', function (res) {
             console.log("rentShepherd Message received: ");
             // Trigger a Unity callback to pass the received message
             unityInstanceRef.SendMessage("SocketIOClient", "HireOnMessageReceived", res);
        });
		
		 socket.on('upgradeConveyor', function (res) {
             console.log("upgradeConveyor Message received: ");
             // Trigger a Unity callback to pass the received message
             unityInstanceRef.SendMessage("SocketIOClient", "LiftOnMessageReceived", res);
        });
		
		 socket.on('upgradeTruck', function (res) {
             console.log("upgradeTruck Message received: ");
             // Trigger a Unity callback to pass the received message
             unityInstanceRef.SendMessage("SocketIOClient", "ShopOnMessageReceived", res);
        });
		
		 socket.on('upgradeSheepFarm', function (res) {
             console.log("upgradeFarmSheep Message received: ");
             // Trigger a Unity callback to pass the received message
             unityInstanceRef.SendMessage("SocketIOClient", "UpgradeFarmOnMessageReceived", res);
        });
		
		 socket.on('activeSheepFarm', function (res) {
             console.log("ActiveFarmOnMessageReceived Message received: ");
             // Trigger a Unity callback to pass the received message
             unityInstanceRef.SendMessage("SocketIOClient", "ActiveFarmOnMessageReceived", res);
        });
		
		 socket.on('boughtSpecialPackage', function (res) {
             //console.log("BuyPackageOnMessageReceived Message received: ");
             // Trigger a Unity callback to pass the received message
             unityInstanceRef.SendMessage("SocketIOClient", "BuyPackageOnMessageReceived", res);
        });
		 socket.on('gameRealTime', function (res) {
             unityInstanceRef.SendMessage("PaymentAnimation", "OnReceiveAccountName", res);
        });
		
		

        socket.on('disconnect', function () {
            console.log("Disconnected from server");
            //SendMessage("GameObjectName", "OnSocketDisconnect", "Disconnected");
        });

        // Store the socket reference in the global scope to maintain its state
        window.unitySocket = socket;
		 
		 
	},  
	// Function to disconnect the socket
    SocketDisconnect: function () {
        if (window.unitySocket) {
            window.unitySocket.disconnect();
            console.log("Socket connection has been disconnected.");
        }
    },
	
	copyText: function (text) {
	
	
 // Chuyển đổi chuỗi từ dạng Unity (Pointer) sang dạng chuỗi của JavaScript
        var jsString = UTF8ToString(text);

        // Kiểm tra nếu đang chạy trên thiết bị iOS
        var isIOS = navigator.userAgent.match(/ipad|iphone/i);
        
        if (isIOS) {
            // Thêm sự kiện touchend để sao chép trên iOS
            document.addEventListener('touchend', function() {
                navigator.clipboard.writeText(jsString).then(function() {
                    console.log("Copied to clipboard successfully on iOS!");
                }).catch(function(err) {
                    console.error("Failed to copy text on iOS: ", err);
                });
            }, { once: true });
        } else {
            // Sao chép text cho các nền tảng khác ngoài iOS
            navigator.clipboard.writeText(jsString).then(function() {
                console.log("Copied to clipboard successfully!");
            }).catch(function(err) {
                console.error("Failed to copy text: ", err);
            });
        }
		
	
	},
	
	
	openLinkTelegramFromUnity: function (link) {
	 var jsLink = UTF8ToString(link);
	 var jsLinkDes = "https://t.me";
  
	 if (window.Telegram && window.Telegram.WebApp && window.Telegram.WebApp.openTelegramLink) {
		 
	  if (jsLink.startsWith(jsLinkDes)) 
	  {
		  
	   window.Telegram.WebApp.openTelegramLink(jsLink);
	  } 
	  else 
	  {
	   window.Telegram.WebApp.openLink(jsLink, {try_instant_view: true});
	  }
	 }
	},
	
	openLinkFromUnity: function (link) {
	var jsLink = UTF8ToString(link);
        window.open(jsLink, "_blank");
	},
});
