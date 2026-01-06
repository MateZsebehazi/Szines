import { Injectable, NgZone } from '@angular/core';
import { environment } from '../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Colors {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient, private ngZone: NgZone) { }

  getColors() {
    return this.http.get<any[]>(this.apiUrl);
  }

  addColor(color: any) {
    return this.http.post(this.apiUrl, color);
  }

  deleteColor(id: number) {
    return this.http.delete(`${this.apiUrl}/${id}`);
  }

  getColorStream(): Observable<string> {
    return new Observable(observer => {
      const eventSource = new EventSource(`${this.apiUrl}/stream`);

      eventSource.onmessage = (event) => {
        this.ngZone.run(() => {
          observer.next(event.data);
        });
      };

      eventSource.onerror = (error) => {
        this.ngZone.run(() => {
          eventSource.close();
          observer.error(error);
        });
      };

      return () => {
        eventSource.close();
      };
    });
  }
}
