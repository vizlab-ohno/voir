#include <stdio.h>
#include <stdlib.h>

#define XSIZE 100
#define YSIZE 100
#define ZSIZE 100

double xmin = -30;
double xmax = 30;

double ymin = -30;
double ymax = 30;

double zmin = 0;
double zmax = 60;

float sigma = 10.0;
float rho = 28.0;
float beta = 8.0/3.0;

/* Coordinates */
double x[XSIZE], y[YSIZE],z[ZSIZE];

/* Data */
float vx[XSIZE*YSIZE*ZSIZE], vy[XSIZE*YSIZE*ZSIZE], vz[XSIZE*YSIZE*ZSIZE];
float e[XSIZE*YSIZE*ZSIZE];
float et[XSIZE*YSIZE*ZSIZE];

char filevx[] = "LorenzVX";
char filevy[] = "LorenzVY";
char filevz[] = "LorenzVZ";
char filee[] = "LorenzE";
char fileet[] = "LorenzET";
char directory[] = "c:\\Users\\ohno\\lorenz\\";

int main(int argc, char **argv)
{
	int i,j,k,p;
	double dx, dy, dz;
	double lenx,leny,lenz;
	int im,ip,jm,jp,km,kp;
	double wx, wy, wz;
	double vzy,vyz, vxz,vzx, vyx,vxy;
	FILE *fp;
	
	dx = (xmax-xmin)/(XSIZE-1);
	dy = (ymax-ymin)/(YSIZE-1);
	dz = (zmax-zmin)/(ZSIZE-1);
	for(i=0;i<XSIZE;i++) x[i] = xmin + dx*i;
	for(j=0;j<YSIZE;j++) y[j] = ymin + dy*j;
	for(k=0;k<ZSIZE;k++) z[k] = zmin + dz*k;
	
	for(k=0;k<ZSIZE;k++){
		for(j=0;j<YSIZE;j++){
			for(i=0;i<XSIZE;i++){
				p = i+j*XSIZE+k*XSIZE*YSIZE; 
				vx[p] = sigma*(y[j]-x[i]);
				vy[p] = x[i]*(rho-z[k]) -y[j];
				vz[p] = x[i]*y[j] - beta*z[k];
				
				e[p] = vx[p]*vx[p] + vy[p]*vy[p] + vz[p]*vz[p];
			}
		}
	}
	
	for(k=0;k<ZSIZE;k++){
		for(j=0;j<YSIZE;j++){
			for(i=0;i<XSIZE;i++){
				p = i+j*XSIZE+k*XSIZE*YSIZE;
				im=i-1;ip=i+1;jp=j+1;jm=j-1;kp=k+1;km=k-1;
				if(i==0){
					vzx = (vz[1+j*XSIZE+k*XSIZE*YSIZE] - vz[j*XSIZE+k*XSIZE*YSIZE])/dx;
					vyx = (vy[1+j*XSIZE+k*XSIZE*YSIZE] - vy[j*XSIZE+k*XSIZE*YSIZE])/dx;
				} else if(i==XSIZE-1){
					vzx = (vz[XSIZE-1+j*XSIZE+k*XSIZE*YSIZE] - vz[XSIZE-2+j*XSIZE+k*XSIZE*YSIZE])/dx;
					vyx = (vy[XSIZE-1+j*XSIZE+k*XSIZE*YSIZE] - vy[XSIZE-2+j*XSIZE+k*XSIZE*YSIZE])/dx;
				} else {
					vzx = (vz[ip +j*XSIZE+k*XSIZE*YSIZE] - vz[im+j*XSIZE+k*XSIZE*YSIZE])/dx/2;
					vyx = (vy[ip +j*XSIZE+k*XSIZE*YSIZE] - vy[im+j*XSIZE+k*XSIZE*YSIZE])/dx/2;
				}
				
				if(j==0){
					vzy = (vz[i+XSIZE+k*XSIZE*YSIZE] - vz[i+k*XSIZE*YSIZE])/dy;
					vxy = (vx[i+XSIZE+k*XSIZE*YSIZE] - vx[i+k*XSIZE*YSIZE])/dy;
				} else if(j==YSIZE-1){
					vzy = (vz[i+(YSIZE-1)*XSIZE+k*XSIZE*YSIZE] - vz[i+(YSIZE-2)*XSIZE+k*XSIZE*YSIZE])/dy;
					vxy = (vx[i+(YSIZE-1)*XSIZE+k*XSIZE*YSIZE] - vx[i+(YSIZE-2)*XSIZE+k*XSIZE*YSIZE])/dy;
				} else {
					vzy = (vz[i+jp*XSIZE+k*XSIZE*YSIZE] - vz[i+jm*XSIZE+k*XSIZE*YSIZE])/dy/2;
					vxy = (vx[i+jp*XSIZE+k*XSIZE*YSIZE] - vx[i+jm*XSIZE+k*XSIZE*YSIZE])/dy/2;
				}
				
				if(k==0){
					vyz = (vy[i+j*XSIZE+XSIZE*YSIZE] - vy[i+j*XSIZE])/dz;
					vxz = (vx[i+j*XSIZE+XSIZE*YSIZE] - vx[i+j*XSIZE])/dz;
				} else if(k==ZSIZE-1){
					vyz = (vy[i+j*XSIZE+(ZSIZE-1)*XSIZE*YSIZE] - vy[i+j*XSIZE+(ZSIZE-2)*XSIZE*YSIZE])/dz;
					vxz = (vx[i+j*XSIZE+(ZSIZE-1)*XSIZE*YSIZE] - vx[i+j*XSIZE+(ZSIZE-2)*XSIZE*YSIZE])/dz;
				}else{
					vyz = (vy[i+j*XSIZE+kp*XSIZE*YSIZE] - vy[i+j*XSIZE+km*XSIZE*YSIZE])/dz/2;
					vxz = (vx[i+j*XSIZE+kp*XSIZE*YSIZE] - vx[i+j*XSIZE+km*XSIZE*YSIZE])/dz/2;
				}
				
				wx = vzy - vyz;
				wy = vxz - vzx;
				wz = vyx - vxy;

				et[p] = wx*wx + wy*wy + wz*wz;
			}
		}
	}

	fp = fopen(filevx, "wb");
        fwrite(vx, sizeof(float), XSIZE*YSIZE*ZSIZE, fp);
        fclose(fp);
        
	fp = fopen(filevy, "wb");
        fwrite(vy, sizeof(float), XSIZE*YSIZE*ZSIZE, fp);
        fclose(fp);

	fp = fopen(filevz, "wb");
        fwrite(vz, sizeof(float), XSIZE*YSIZE*ZSIZE, fp);
        fclose(fp);

	fp = fopen(filee, "wb");
        fwrite(e, sizeof(float), XSIZE*YSIZE*ZSIZE, fp);
        fclose(fp);

	fp = fopen(fileet, "wb");
        fwrite(et, sizeof(float), XSIZE*YSIZE*ZSIZE, fp);
        fclose(fp);

	fp = fopen("lorenz.jv", "w");
	fprintf(fp, "#VOIR configuration file for Lorenz Attractor\n");
	fprintf(fp, "GRIDSIZE %d %d %d\n",XSIZE,YSIZE,ZSIZE);
	fprintf(fp, "\n");
	fprintf(fp, "NVEC 1\n");
	fprintf(fp, "NSCAL 2\n");
	fprintf(fp, "\n#COORDINATES\n");
	fprintf(fp, "UNIFORM\n");
	lenx = xmax-xmin; leny = ymax-ymin; lenz = zmax-zmin;
	fprintf(fp, "DX %f %f %f\n",dx/lenx, dy/leny, dz/lenz);
	fprintf(fp, "CORNER %f %f %f\n",xmin/lenx, ymin/leny, zmin/lenz);
	fprintf(fp, "\n#DATA\n");
	fprintf(fp, "SINGLEPRECISION\n\n");
	fprintf(fp, "#Vector Data\n");
	fprintf(fp, "VECT0_LABEL Vector\n");
	fprintf(fp, "VECT0X  %s%s\n",directory,filevx);
	fprintf(fp, "VECT0Y  %s%s\n",directory,filevy);
	fprintf(fp, "VECT0Z  %s%s\n",directory,filevz);
	
	fprintf(fp, "\n#Scalar Data\n");
	fprintf(fp, "SCAL0_LABEL Scalar1\n");
	fprintf(fp, "SCAL0  %s%s\n\n",directory,filee);
	fprintf(fp, "SCAL1_LABEL Scalar2\n");
	fprintf(fp, "SCAL1  %s%s\n\n",directory,fileet);
	
	fclose(fp);	

	return 0;
}

