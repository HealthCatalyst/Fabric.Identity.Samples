import { Injectable } from '@angular/core';

@Injectable()
export class LoggingService {

  constructor() { }

  messages: string[] = [];

  debug(...logmessages) {
    console.debug(logmessages);
    this.processMessages(logmessages);
  }

  log(...logmessages) {
    console.log(logmessages);
    this.processMessages(logmessages);
  }

  warn(...logmessages) {
    console.warn(logmessages);
    this.processMessages(logmessages);
  }

  error(...logmessages) {
    console.error(logmessages);
    this.processMessages(logmessages);
  }

  processMessages(...logmessages) {
    for (var message of logmessages) {
      if (typeof message === "object") {
        message = JSON.stringify(message)
      }
      this.writeToNavPane(message);
    }
  }

  writeToNavPane(message: string) {
    this.messages.push(message);
  }
}
