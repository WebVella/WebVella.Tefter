DROP TABLE IF EXISTS rel_data;
DROP TABLE IF EXISTS adapter_data;
DROP TABLE IF EXISTS app_data;


CREATE TABLE adapter_data ( id UUID PRIMARY KEY, name VARCHAR(50) NOT NULL );
CREATE TABLE app_data ( id UUID PRIMARY KEY, name VARCHAR(50) NOT NULL );

CREATE TABLE rel_data ( 
	adapter_data_id UUID REFERENCES adapter_data(id),
	app_data_id UUID REFERENCES app_data(id),
	CONSTRAINT rel_data_pk PRIMARY KEY(adapter_data_id,app_data_id) 
);


INSERT INTO adapter_data VALUES( '10000000-0000-40cd-8bde-01beb34c398a', 'adapter data 1');
INSERT INTO adapter_data VALUES( '20000000-0000-40cd-8bde-01beb34c398a', 'adapter data 2');
INSERT INTO adapter_data VALUES( '30000000-0000-40cd-8bde-01beb34c398a', 'adapter data 3');
INSERT INTO adapter_data VALUES( '40000000-0000-40cd-8bde-01beb34c398a', 'adapter data 4');
INSERT INTO adapter_data VALUES( '50000000-0000-40cd-8bde-01beb34c398a', 'adapter data 5');

INSERT INTO app_data VALUES( '10000000-1111-40cd-8bde-01beb34c398a', 'app data 1');
INSERT INTO app_data VALUES( '20000000-1111-40cd-8bde-01beb34c398a', 'app data 2');
INSERT INTO app_data VALUES( '30000000-1111-40cd-8bde-01beb34c398a', 'app data 3');
INSERT INTO app_data VALUES( '40000000-1111-40cd-8bde-01beb34c398a', 'app data 4');
INSERT INTO app_data VALUES( '50000000-1111-40cd-8bde-01beb34c398a', 'app data 5');

INSERT INTO app_data VALUES( '62000000-1111-40cd-8bde-01beb34c398a', 'app data 6 2'); 
INSERT INTO app_data VALUES( '63000000-1111-40cd-8bde-01beb34c398a', 'app data 6 3');
INSERT INTO app_data VALUES( '64000000-1111-40cd-8bde-01beb34c398a', 'app data 6 4');

INSERT INTO app_data VALUES( '74000000-1111-40cd-8bde-01beb34c398a', 'app data 7 4');
INSERT INTO app_data VALUES( '75000000-1111-40cd-8bde-01beb34c398a', 'app data 7 5');

INSERT INTO app_data VALUES( '82000000-1111-40cd-8bde-01beb34c398a', 'app data 8 2');
INSERT INTO app_data VALUES( '84000000-1111-40cd-8bde-01beb34c398a', 'app data 8 4');


INSERT INTO rel_data VALUES( '10000000-0000-40cd-8bde-01beb34c398a', '10000000-1111-40cd-8bde-01beb34c398a');
INSERT INTO rel_data VALUES( '20000000-0000-40cd-8bde-01beb34c398a', '20000000-1111-40cd-8bde-01beb34c398a');
INSERT INTO rel_data VALUES( '30000000-0000-40cd-8bde-01beb34c398a', '30000000-1111-40cd-8bde-01beb34c398a');
INSERT INTO rel_data VALUES( '40000000-0000-40cd-8bde-01beb34c398a', '40000000-1111-40cd-8bde-01beb34c398a');
INSERT INTO rel_data VALUES( '50000000-0000-40cd-8bde-01beb34c398a', '50000000-1111-40cd-8bde-01beb34c398a');

INSERT INTO rel_data VALUES( '20000000-0000-40cd-8bde-01beb34c398a', '62000000-1111-40cd-8bde-01beb34c398a');
INSERT INTO rel_data VALUES( '30000000-0000-40cd-8bde-01beb34c398a', '63000000-1111-40cd-8bde-01beb34c398a');

INSERT INTO rel_data VALUES( '40000000-0000-40cd-8bde-01beb34c398a', '74000000-1111-40cd-8bde-01beb34c398a');
INSERT INTO rel_data VALUES( '50000000-0000-40cd-8bde-01beb34c398a', '75000000-1111-40cd-8bde-01beb34c398a');

INSERT INTO rel_data VALUES( '20000000-0000-40cd-8bde-01beb34c398a', '82000000-1111-40cd-8bde-01beb34c398a');
INSERT INTO rel_data VALUES( '40000000-0000-40cd-8bde-01beb34c398a', '84000000-1111-40cd-8bde-01beb34c398a');


WITH 
	app_adapter_filter AS (
		SELECT DISTINCT 
			ad.id,
			CASE WHEN app.id IS NULL THEN FALSE ELSE TRUE END AS Matched
		FROM adapter_data ad
			LEFT JOIN rel_data AS rd ON rd.adapter_data_id = ad.id 
			LEFT JOIN app_data AS app ON app.id = rd.app_data_id AND app.name ILIKE 'app data %'
	),
	app_data_filter AS (
		SELECT  
			ad.id AS adapter_id,
	 		json_agg(row_to_json(app)) AS json_data,
			-- can be MIN,MAX,COUNT - select min,max or count value of multiple records related to adapter id
			MAX ( app.name )  AS order_column 
		FROM adapter_data AS ad 
			LEFT JOIN rel_data AS rd ON ad.id = rd.adapter_data_id 
			LEFT JOIN app_data AS app ON rd.app_data_id = app.id AND app.name ILIKE 'app data %'
		WHERE app.id IS NOT NULL
		GROUP BY ad.id
	)
SELECT 
	ad.id,
	ad.name,
	df.json_data
FROM adapter_data ad
	INNER JOIN app_adapter_filter af ON ad.id = af.id AND af.Matched
	INNER JOIN app_data_filter df ON df.adapter_id = af.id 
ORDER BY df.order_column ASC;

