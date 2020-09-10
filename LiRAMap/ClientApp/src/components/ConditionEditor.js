import React, { PureComponent } from 'react';
import { Button, Form, FormGroup, Label, Input, FormText, InputGroup, InputGroupAddon } from 'reactstrap';
import { Condition } from '../Classes';

export class ConditionEditor extends PureComponent {
	constructor(props) {
		super(props);
		this.state = {
			condition: {
				id: props.condition.id,
				type: props.condition.type,
				severity: props.condition.severity,
				timestamp: props.condition.timestamp,
				ways: props.condition.ways,
			}
		}
	}

	render() {
		return (
			<Form>
				{id
					? <Label>Editing Condition [{id}]</Label>
					: <Label>Creating New Condition</Label>
				}
				<FormGroup inline>
					<Label for="type">Condition Type</Label>
					<InputGroup>
						<InputGroupAddon addonType="prepend">
							<Input type="select" name="select" id="type" placeholder="Condition Type">
								{Condition.ConditionTypes.map((t, k) =>
									<option value={k} selected={k == this.state.condition.type}>{t.Name}</option>
								)}
							</Input>
						</InputGroupAddon>
						<Input placeholder="Severity" min={0} type="number" step="1" value={this.state.condition.severity}/>
					</InputGroup>
				</FormGroup>
				<FormGroup>

				</FormGroup>
			</Form>
		);
	}
}