import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ColorManager } from './color-manager';

describe('ColorManager', () => {
  let component: ColorManager;
  let fixture: ComponentFixture<ColorManager>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ColorManager]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ColorManager);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
