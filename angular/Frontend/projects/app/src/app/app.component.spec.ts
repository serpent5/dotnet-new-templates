import { ComponentFixture, TestBed } from "@angular/core/testing";
import { AppComponent } from "./app.component";

describe("AppComponent", () => {
  let componentFixture: ComponentFixture<AppComponent>;
  let componentInstance: AppComponent;

  beforeEach(() => {
    componentFixture = TestBed.createComponent(AppComponent);
    componentInstance = componentFixture.componentInstance;
  });

  it("should create the app", () => expect(componentInstance).toBeTruthy());

  it("should render title", () => {
    componentFixture.detectChanges();

    const el = componentFixture.nativeElement as HTMLElement;
    expect(el.querySelector("h1")?.textContent).toContain("Angular_Template");
  });
});
