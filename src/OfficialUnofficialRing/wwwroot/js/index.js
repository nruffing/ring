(function () {
    new Vue({
        el: "#app",
        data: {
            devices: null,
            deviceIds: null,
        },
        mounted: function () {
            const self = this;
            fetch("/device")
                .then(response => response.json())
                .then(data => {
                    data.forEach(device => device.imgSrc = null);
                    self.devices = data;
                    this.deviceIds = [];
                    this.devices.forEach(device => this.deviceIds.push(device.id))
                    self.refreshSnapshots();
                    setInterval(self.refreshSnapshots, 20000);
                });
        },
        methods: {
            refreshSnapshots: function () {
                const self = this;

                if (!this.devices) {
                    return
                }

                fetch("/snapshot/update", {
                    method: "POST",
                    body: JSON.stringify({ DeviceIds: this.deviceIds }),
                    headers: {
                        'Content-Type': 'application/json'
                    }
                })
                    .then(response => {
                        setTimeout(() => {
                            self.devices.forEach(device => {
                                fetch("/snapshot/device/" + device.id)
                                    .then(response => response.json())
                                    .then(data => device.imgSrc = "data:image/jpg;base64," + data.rawJpg);
                            });
                        }, 2000);
                    });                
            }
        },
    });
})();