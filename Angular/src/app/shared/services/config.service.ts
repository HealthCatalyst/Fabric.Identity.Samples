import { Inject, Injectable } from '@angular/core';
import { Http } from '@angular/http';
import { Observable } from 'rxjs/Rx';
import { Config } from '../../models/config';

@Injectable()
export class ConfigService{
    config: Config;

    constructor(private http: Http){
        this.config = new Config();        
    }

    loadConfig(){
        return new Promise((resolve, reject) => {
            this.http.get('assets/appconfig.json')
            .map(res => <Config>res.json() )
            .subscribe(config => {
                console.log('configuration loaded......');
                this.config = config;
                resolve();
            });        
        });
    }

}