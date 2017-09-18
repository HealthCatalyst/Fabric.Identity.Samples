import { Component } from '@angular/core';
import { HttpInterceptorService } from 'ng-http-interceptor';
import { LoggingService } from 'app/shared/services/logging.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {

  constructor(private _httpInterceptor: HttpInterceptorService, private _loggingService: LoggingService){
    _httpInterceptor.request().addInterceptor((data, method) => {
     
      if(typeof(data[1]) === 'object'){
        var requestObject = data[1];
        requestObject.method = method;
        requestObject.url = data[0];

        data[1] = requestObject;
      }
      
      _loggingService.log(data);
      return data;
    });    

    _httpInterceptor.response().addInterceptor((res, method) => {
      return res.do(r => _loggingService.log(r));
    });
  }

  formatResponse(res, method){
    
  }

}
