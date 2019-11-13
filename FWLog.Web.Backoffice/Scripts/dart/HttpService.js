
class HttpService {

    get(url, data, global, beforeSend, complete) {

        if (beforeSend == null) {
            beforeSend = () => { };
        }

        if (complete == null) {
            complete = () => { };
        }

        return new Promise((resolve, reject) => {
            $.ajax({
                url: url,
                method: "GET",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: data,
                beforeSend: () => beforeSend(),
                success: response => resolve(response),
                complete: () => complete(),
                error: responseError => reject(responseError),
                global: global,
            });
        });
    }


    post(url, data, global, beforeSend, complete) {

        if (beforeSend == null) {
            beforeSend = () => { };
        }

        if (complete == null) {
            complete = () => { };
        }



        return new Promise((resolve, reject) => {
            $.ajax({
                url: url,
                method: "POST",
                dataType: "json",
                contentType: "application/json",
                data: JSON.stringify(data),
                beforeSend: () => beforeSend(),
                success: response => resolve(response),
                complete: () => complete(),
                error: responseError => reject(responseError),
                global: global,
            });
        });
    }
}
