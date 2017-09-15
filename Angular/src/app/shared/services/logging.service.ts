import { Injectable } from '@angular/core';

@Injectable()
export class LoggingService {

  constructor() { }

  messages: string[] = [];

  debug(message: string) {
    console.debug(message);
    this.writeToNavPane(message);
  }

  log(message: string) {
    console.log(message);
    this.writeToNavPane(message);
  }

  warn(message: string) {
    console.warn(message);
    this.writeToNavPane(message);
  }

  error(message: string) {
    console.error(message);
    this.writeToNavPane(message);
  }

  writeToNavPane(message: string) {
    this.messages.push(message);
  }
}
